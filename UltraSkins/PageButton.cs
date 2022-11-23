using System;
using System.IO;
using UnityEngine;


namespace UltraSkins
{
    public class PageButton : MonoBehaviour
    {
        public GameObject Activator;
        public ULTRASKINHand UKSH;
        public PageEventHandler pageEventHandler;
        public int moveamount;

        private void Update()
        {
            if (Activator != null && Activator.activeSelf)
            {
                Activator.SetActive(false);
                pageEventHandler.pageNumber += moveamount;
                pageEventHandler.UpdatePage();
            }
        }
    }
}
