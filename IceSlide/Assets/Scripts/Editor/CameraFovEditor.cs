using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[InitializeOnLoad]
public static class CameraFovEditor
{
    static Camera mainCamera;
    static Camera playerCamera;

    static CameraFovEditor()
    {
        mainCamera = Camera.main;
        foreach (Transform item in mainCamera.transform)
        {
            if(item.TryGetComponent<Camera>(out playerCamera))
            {
                break;
            }
        }
       
        ObjectChangeEvents.changesPublished += ObjectChangeEvents_changesPublished;
    }
    
    private static void ObjectChangeEvents_changesPublished(ref ObjectChangeEventStream stream)
    {
        for (int i = 0; i < stream.length; i++)
        {
            if (stream.GetEventType(i) == ObjectChangeKind.ChangeGameObjectOrComponentProperties)
            {
                stream.GetChangeGameObjectOrComponentPropertiesEvent(i, out ChangeGameObjectOrComponentPropertiesEventArgs changeArgs);
                UnityEngine.Object obj = EditorUtility.InstanceIDToObject(changeArgs.instanceId);
                if (obj is Camera camera)
                {
                    // you can then see what camera it was and copy the value from one to another.
                    if (camera.Equals(mainCamera))
                    {
                        playerCamera.orthographicSize = mainCamera.orthographicSize;
                    }
                    else if (camera.Equals(playerCamera))
                    {
                        mainCamera.orthographicSize = playerCamera.orthographicSize;
                    }
                }
            }
        }
    }
}
