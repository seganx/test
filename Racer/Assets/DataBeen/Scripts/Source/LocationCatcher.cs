using UnityEngine;
using System.Collections;
using System;
namespace DataBeenConnection
{
    public class LocationCatcher : MonoBehaviour
    {

        public event Action<string> OnLocationCatched;

        public void SetMethod(Action<string> doneMethod)
        {
            OnLocationCatched += doneMethod;
        }

        // Use this for initialization
        void Start()
        {
            OnLocationCatched += DestroySelf;
            StartCoroutine(_GetLocation());
        }

        private string location = "0,0";

        IEnumerator _GetLocation()
        {
            yield return new WaitForSeconds(.1f);
            if (OnLocationCatched != null)
                OnLocationCatched(location);
        }

        public void DestroySelf(string s)
        {
            Destroy(gameObject, 1);
        }
    }
}