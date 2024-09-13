using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Andrius.Core.Utils
{
    public static class CoroutineUtils
    {

        public static IEnumerator DelayedAction(float time, Action action, bool scaledTime = true)
        {
            if (scaledTime)
            {
                yield return new WaitForSeconds(time);
            }
            else
            {
                yield return new WaitForSecondsRealtime(time);
            }
            action?.Invoke();
        }

        public static IEnumerator SendWebRequest(UnityWebRequest request, Action<string> succesCallback, Action<string> errorCallback)
        {
            yield return request.SendWebRequest();

            bool isError = request.result == UnityWebRequest.Result.ConnectionError || request.downloadHandler == null;

            if (isError)
            {
                string result = "";
                if(request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    result = request.downloadHandler.text;
                }
                else
                {
                    result = request.error;
                }
                errorCallback?.Invoke(result);
            }
            else
            {
                succesCallback?.Invoke(request.downloadHandler.text);
            }
            request.Dispose();
        }
    }
}
