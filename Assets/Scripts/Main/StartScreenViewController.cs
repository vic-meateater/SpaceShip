using Main;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenViewController : MonoBehaviour
{
    [SerializeField] private Button _serverModeButton;
    [SerializeField] private Button _clientModeButton;
    [SerializeField] private TextMeshProUGUI _modeTypeText;

    [SerializeField] private GameObject _serverOptions;
    [SerializeField] private GameObject _clientOptions;

    [SerializeField] private Button _serverEnterOptionsButton;
    [SerializeField] private Button _serverBackStepButton;
    [SerializeField] private Button _launchServerButton;
    [SerializeField] private Slider _countSlider;
    [SerializeField] private Slider _radiusSlider;
    [SerializeField] private TextMeshProUGUI _countSliderValueText;
    [SerializeField] private TextMeshProUGUI _radiusSliderValueText;

    [SerializeField] private Button _clientEnterOptionsButton;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _clientBackStepButton;
    [SerializeField] private Button _launchClientButton;

    [SerializeField] private SolarSystemNetworkManager _systemNetworkManager;

    private void Start()
    {
        _serverModeButton.onClick.AddListener(OpenServerOptions);
        _clientModeButton.onClick.AddListener(OpenClientOptions);
        _serverBackStepButton.onClick.AddListener(CloseOptionsPanels);
        _clientBackStepButton.onClick.AddListener(CloseOptionsPanels);
        _serverEnterOptionsButton.onClick.AddListener(SetServerOptions);
        _clientEnterOptionsButton.onClick.AddListener(SetClientOptions);
        _countSlider.onValueChanged.AddListener((_) =>
                _countSliderValueText.text = ((int)_countSlider.value).ToString());
        _radiusSlider.onValueChanged.AddListener((_) =>
                _radiusSliderValueText.text = ((int)_radiusSlider.value).ToString());
        _launchServerButton.onClick.AddListener(() =>
        {
            _systemNetworkManager.StartServer();
            gameObject.SetActive(false);
        });
        _launchClientButton.onClick.AddListener(() =>
        {
            _systemNetworkManager.StartClient();
            gameObject.SetActive(false);
        });
        _countSliderValueText.text = ((int)_countSlider.value).ToString();
        _radiusSliderValueText.text = ((int)_radiusSlider.value).ToString();
    }

    private void OpenServerOptions()
    {
        _serverOptions.SetActive(true);
        _clientOptions.SetActive(false);
        _modeTypeText.text = "Server";
        _launchServerButton.interactable = false;
        _serverModeButton.gameObject.SetActive(false);
        _clientModeButton.gameObject.SetActive(false);
    }

    private void OpenClientOptions()
    {
        _serverOptions.SetActive(false);
        _clientOptions.SetActive(true);
        _modeTypeText.text = "Client";
        _launchClientButton.interactable = false;
        _serverModeButton.gameObject.SetActive(false);
        _clientModeButton.gameObject.SetActive(false);
    }

    private void CloseOptionsPanels()
    {
        _serverOptions.SetActive(false);
        _clientOptions.SetActive(false);
        _modeTypeText.text = "";
        _serverModeButton.gameObject.SetActive(true);
        _clientModeButton.gameObject.SetActive(true);
    }

    private void SetServerOptions()
    {
        _systemNetworkManager.SetServerOptions((int)_countSlider.value, _radiusSlider.value);
        _launchServerButton.interactable = true;
    }
    private void SetClientOptions()
    {
        _systemNetworkManager.SetClientOptions(_inputField.text);
        _launchClientButton.interactable = true;
    }

    private void OnDestroy()
    {
        _serverModeButton.onClick.RemoveAllListeners();
        _clientModeButton.onClick.RemoveAllListeners();
        _serverBackStepButton.onClick.RemoveAllListeners();
        _clientBackStepButton.onClick.RemoveAllListeners();
        _serverEnterOptionsButton.onClick.RemoveAllListeners();
        _clientEnterOptionsButton.onClick.RemoveAllListeners();
        _countSlider.onValueChanged.RemoveAllListeners();
        _radiusSlider.onValueChanged.RemoveAllListeners();
        _launchServerButton.onClick.RemoveAllListeners();
        _launchClientButton.onClick.RemoveAllListeners();
    }
}
