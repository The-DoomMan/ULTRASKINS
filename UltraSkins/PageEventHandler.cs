using System;
using UnityEngine;

namespace UltraSkins
{ 
    public class PageEventHandler : MonoBehaviour
    {
        public ULTRASKINHand UKSH;
        public float pageNumber = 0;
        public float pagesamount;

        public void UpdatePage()
        {
            if (pageNumber < 0)
                pageNumber = 0;
            else if (pageNumber > pagesamount)
                pageNumber = pagesamount;

                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                transform.GetChild((int)Mathf.Round(pageNumber)).gameObject.SetActive(true);
        }
    }
}
