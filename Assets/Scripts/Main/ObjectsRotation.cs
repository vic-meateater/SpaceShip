using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Main
{
    public class ObjectsRotation
    {
        private TransformAccessArray _transformsArray;
        private NativeArray<float> _angles;
        private List<Transform> _objects;
        private bool _isReadyRotate;

        public ObjectsRotation(List<Transform> objects)
        {
            _objects = objects;
            PrepairIJobRotationTask();
        }

        private void PrepairIJobRotationTask()
        {
            _angles = new NativeArray<float>(_objects.Count, Allocator.Persistent);
            _transformsArray = new TransformAccessArray(_objects.ToArray());
            _isReadyRotate = true;
        }

        public void StartIJobRotationTask(float deltaTime)
        {
            if (!_isReadyRotate) return;

            RotationJobStruct rotationJobStruct = new RotationJobStruct()
            {
                Angles = _angles,
                Deltatime = deltaTime
            };

            JobHandle jobHandle = rotationJobStruct.Schedule(_transformsArray);
            jobHandle.Complete();
        }

        public struct RotationJobStruct : IJobParallelForTransform
        {
            public NativeArray<float> Angles;
            public float Deltatime;

            public void Execute(int index, TransformAccess transform)
            {
                transform.rotation = Quaternion.AngleAxis(Angles[index], Vector3.up);
                Angles[index] = Angles[index] == 360 ? 0 : Angles[index] + 50 * Deltatime;
            }
        }

        public void CleanUp()
        {
            _transformsArray.Dispose();
            _angles.Dispose();
        }
    }
}