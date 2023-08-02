using Characters;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Main
{
    public partial class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject _crystallPrefab;
        [SerializeField] private TextMeshProUGUI _currentCrystallsCountTitle;
        [SerializeField] private TextMeshProUGUI _currentRemainingCountTitle;
        [SerializeField] private TextMeshProUGUI _currentCrystallsCountText;
        [SerializeField] private TextMeshProUGUI _currentRemainingCountText;
        [SerializeField] private GameObject _leaderBoard;
        [SerializeField] private TextMeshProUGUI _leaderBoardNameText;
        [SerializeField] private TextMeshProUGUI _leaderBoardScoreText;
        [SerializeField] private List<ObjectsRotation> _objectsForRotate;

        private int _remainingCrystallsCount;
        private int _currentCrystallsCount;
        private int _playerIDOnServer;
        private Dictionary<int, ShipController> _players;
        private List<Transform> _crystalls;
        private GameObject _crystallsHolder;
        private bool _isServer;
        private float _spawnRadius;
        private int _crystallsCount;
        private string _enteredLogin;
        private string _playerName;
               
        public void SetServerOptions(int count, float radius)
        {
            _crystallsCount = count;
            _spawnRadius = radius;
        }

        public void SetClientOptions(string login)
        {
            _enteredLogin = login;
        }

        private void Update()
        {
            if (_isServer) return;

            if(_objectsForRotate != null && _objectsForRotate.Count > 0)
            {
                foreach (var controller in _objectsForRotate)
                {
                    controller.StartIJobRotationTask(Time.deltaTime);
                }
            }            
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            var shipController = player.GetComponent<ShipController>();
            shipController.PlayerName = _playerName;
            shipController.InitPlayer(conn.connectionId);
            shipController.OnCollideWithCrystall += OnDestroyCrystall;
            Debug.Log(conn.connectionId);
            _players.Add(conn.connectionId, shipController);

            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

            MessageInt serverID = new MessageInt()
            {
                ID = conn.connectionId
            };
            NetworkServer.SendToClient(conn.connectionId, 203, serverID);

            for (int i = 0; i < _crystalls.Count; i++)
            {
                MessageCrystallPos crystall = new MessageCrystallPos
                {
                    CrystallPos = _crystalls[i].position
                };
                NetworkServer.SendToClient(conn.connectionId, 200, crystall);
            }

            MessageInt onCompleteObjectsGeneration = new MessageInt()
            {
                ID = conn.connectionId
            };
            NetworkServer.SendToClient(conn.connectionId, 206, onCompleteObjectsGeneration);           
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            _isServer = true;
            _players = new Dictionary<int, ShipController>();
            _crystalls = new List<Transform>();
            CreateCrystalls();
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            NetworkServer.RegisterHandler(100, SendName); 
        }  
               
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            client.RegisterHandler(200, CreateCrystall);
            client.RegisterHandler(201, ClientRefreshCounts);
            client.RegisterHandler(202, DeleteCrystall);
            client.RegisterHandler(203, SetServerID);
            client.RegisterHandler(204, ClientSetLeaderBoardName);
            client.RegisterHandler(205, ClientSetLeaderBoardScore);
            client.RegisterHandler(206, StartObjectsRotation);
            Debug.Log(conn.connectionId);
            MessageLogin login = new MessageLogin();
            login.login = _enteredLogin;
            conn.Send(100, login);
        }

        public override void OnStartClient(NetworkClient client)
        {
            base.OnStartClient(client);
            _isServer = false;
            _currentCrystallsCountText.enabled = true;
            _currentCrystallsCountText.text = 0.ToString();
            _leaderBoardNameText.text = "";
            _leaderBoardScoreText.text = "";
            _remainingCrystallsCount = 0;
            _currentCrystallsCountTitle.enabled = true;
            _currentRemainingCountTitle.enabled = true;
            _currentCrystallsCountText.enabled = true;
            _currentRemainingCountText.enabled = true;
        }

        public void SendName(NetworkMessage networkMessage)
        {
            _players[networkMessage.conn.connectionId].PlayerName = networkMessage.reader.ReadString();
            _players[networkMessage.conn.connectionId].gameObject.name = _players[networkMessage.conn.connectionId].PlayerName;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            _players.Clear();
            foreach (var crystall in _crystalls)
            {
                Destroy(crystall.gameObject);
            }
            _crystalls.Clear();
        }

        private void CreateCrystall(NetworkMessage networkMessage)
        {
            if(_crystalls == null)
            {
                _crystalls = new List<Transform>();
            }

            if(_crystallsHolder == null)
            {
                _crystallsHolder = new GameObject("CrystallsHolder");
            }

            var examplePosition = networkMessage.reader.ReadVector3();

            var crystall = Instantiate(_crystallPrefab, _crystallsHolder.transform);
            crystall.transform.position = examplePosition;
            crystall.transform.localScale = new Vector3(10, 10, 10);
            _crystalls.Add(crystall.transform);
            _remainingCrystallsCount++;
            _currentRemainingCountText.text = _remainingCrystallsCount.ToString();
            //var exampleGO = networkMessage.reader.ReadGameObject();
            //Debug.Log(exampleGO);
            //exampleGO.transform.parent = _crystallsHolder.transform;           
        }

        private void CreateCrystalls()
        {
            _crystallsHolder = new GameObject("CrystallsHolder");

            for (int i = 0; i < _crystallsCount; i++)
            {
                var crystallTransform = Instantiate(_crystallPrefab, _crystallsHolder.transform).transform;
                crystallTransform.localScale = new Vector3(10, 10, 10);
                crystallTransform.position = Random.insideUnitSphere* _spawnRadius;
                _crystalls.Add(crystallTransform);
            }
        }    
         
        public void OnDestroyCrystall(int connID, Transform crystallTransform)
        {
            var crystall = _crystalls.Find(crystall => crystall == crystallTransform);

            if(crystall != null)
            {
                var index = _crystalls.IndexOf(crystall);

                MessageInt crystallID = new MessageInt()
                {
                    ID = index
                };
                NetworkServer.SendToAll(202, crystallID);

                Destroy(_crystalls[index].gameObject);
                _crystalls.Remove(_crystalls[index]);

                RefreshCounts(connID);

                Debug.Log($"sendToAll {connID}");
            }
            
            if (_crystalls.Count == 0)
            {
                SetLeaderBoard();
            }

        }

        private void RefreshCounts(int index)
        {
            _players[index].RpcIncreaseCrystallsCount();
            _players[index].IncreaseCrystallsCount();

            MessageInt clientID = new MessageInt()
            {
                ID = index
            };
            NetworkServer.SendToAll(201, clientID);
        }

        private void ClientRefreshCounts(NetworkMessage networkMessage)
        { 
            if (_playerIDOnServer == networkMessage.reader.ReadInt16())
            {               
                _currentCrystallsCount++;
                _currentCrystallsCountText.text = _currentCrystallsCount.ToString();
            }
            _remainingCrystallsCount--;
            _currentRemainingCountText.text = _remainingCrystallsCount.ToString();
        }

        public void DeleteCrystall(NetworkMessage networkMessage)
        {
            var index = networkMessage.reader.ReadInt16();
            Destroy(_crystalls[index].gameObject);
            _crystalls.Remove(_crystalls[index]);
            Debug.Log("delete");
        }

        public void SetServerID(NetworkMessage networkMessage)
        {
            _playerIDOnServer = networkMessage.reader.ReadInt16();
        }

        private void StartObjectsRotation(NetworkMessage networkMessage)
        {
            if (_objectsForRotate == null)
            {
                _objectsForRotate = new List<ObjectsRotation>(_crystalls.Count);
            }

            var rotationController = new ObjectsRotation(_crystalls);
            _objectsForRotate.Add(rotationController);
        }

        private void SetLeaderBoard()
        {
            foreach (var pair in _players)
            {
                var player = pair.Value;

                MessageLogin playerName = new MessageLogin()
                {
                    login = player.PlayerName
                };
                NetworkServer.SendToAll(204, playerName);

                MessageInt playerScore = new MessageInt()
                {
                    ID = player.CrystallsCount
                };
                NetworkServer.SendToAll(205, playerScore);
            }
        }

        private void ClientSetLeaderBoardName(NetworkMessage networkMessage)
        {
            if (!_leaderBoard.activeInHierarchy)
            {
                _leaderBoard.SetActive(true);
            }

            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }

            var name = networkMessage.reader.ReadString();
            _leaderBoardNameText.text = _leaderBoardNameText.text + name + "\n";
        }

        private void ClientSetLeaderBoardScore(NetworkMessage networkMessage)
        {
            var score = networkMessage.reader.ReadInt16();
            _leaderBoardScoreText.text = _leaderBoardScoreText.text + score + "\n";
        }

        private void OnDestroy()
        {
            if(_objectsForRotate != null)
            {
                foreach (var controller in _objectsForRotate)
                {
                    controller.CleanUp();
                }
            }
        }
    }
}
