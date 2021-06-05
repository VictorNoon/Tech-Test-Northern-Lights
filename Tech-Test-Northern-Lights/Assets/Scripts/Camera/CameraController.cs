using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NLTechTest.Camera
{
    public class CameraController : MonoBehaviour
    {
        //Serialized Fields
        //Camera Motion
        

        //Camera Zoom
        [SerializeField]
        private float speed = 5f;
        [SerializeField]
        private float zoomSpeed = 0;
        [SerializeField]
        private float maxElevation = 100;
        [SerializeField]
        private float minElevation = 0.1f;
        [SerializeField]
        LODGroup lODGroup;
        [SerializeField]
        List<float> lODForcedElevations;

        //Unserialized Fields
        //Camera Motion
        private Vector3 rawInput;
        private float zoomInput;
        public void OnMove(InputAction.CallbackContext value)
        {
            Vector2 inputMovement = value.ReadValue<Vector2>();
            UpdateMotionInput(inputMovement);
        }

        public void OnZoom(InputAction.CallbackContext value)
        {
            zoomInput = value.ReadValue<float>();
        }

        private void UpdateMotionInput(Vector2 inputMovement)
        {
            rawInput = new Vector3(inputMovement.x, 0, inputMovement.y);
        }

        private void UpdateCameraElevation()
        {
            if (zoomInput == 0)
                return;
            
            transform.position += Vector3.down * zoomInput * zoomSpeed * Time.deltaTime;
            if (transform.position.y > maxElevation)
                transform.position = new Vector3(transform.position.x, maxElevation, transform.position.z);
            if (transform.position.y < minElevation)
                transform.position = new Vector3(transform.position.x, minElevation, transform.position.z);
        }

        private void UpdatePosition()
        {
            transform.Translate(rawInput * Time.deltaTime * speed, Space.World);
        }
        
        private void UpdateLODGroup()
        {
            for (int i = 0; i < lODForcedElevations.Count; i++)
            {
                if (transform.position.y > lODForcedElevations[i]) {
                    lODGroup.ForceLOD(lODGroup.lodCount - i - 1);
                    break;
                }
                    
            }
        }

        private void Update()
        {
            UpdateCameraElevation();
            UpdatePosition();
            UpdateLODGroup();
        }


    }
}

