using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SeganX
{
    [DefaultExecutionOrder(1)]
    public class TextTyper : MonoBehaviour
    {
        [SerializeField] private Text target = null;
        [SerializeField] private float delayTime = 0.035f;

        private string[] lines = null;
        private string text = string.Empty;
        private WaitForSeconds typeDelay = null;


        private void Awake()
        {
            typeDelay = new WaitForSeconds(delayTime);
        }

        private bool IsChanged
        {
            get
            {
                var currtext = target.text;
                if (currtext.Length > text.Length) return true;
                for (int i = currtext.Length - 1; i >= 0; i--)
                    if (currtext[currtext.Length - i - 1] != text[text.Length - i - 1])
                        return true;
                return false;
            }
        }

        private void LateUpdate()
        {
            if (IsChanged)
            {
                text = target.text;
                lines = text.Split('\n');
                target.text = string.Empty;
                StopAllCoroutines();
                StartCoroutine(TypeText());
            }
        }

        private IEnumerator TypeText()
        {
            for (int l = 0; l < lines.Length; l++)
            {
                for (int i = lines[l].Length - 1; i >= 0; i--)
                {
                    target.text = lines[l][i] + target.text;
                    yield return typeDelay;
                }
            }
        }
    }
}
