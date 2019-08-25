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

        private string text = string.Empty;
        private WaitForSecondsRealtime typeDelay = null;

        private void Awake()
        {
            typeDelay = new WaitForSecondsRealtime(0.03f);
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
                target.text = string.Empty;
                StopAllCoroutines();
                StartCoroutine(TypeText());
            }
        }

        private IEnumerator TypeText()
        {
            for (int i = text.Length - 1; i >= 0; i--)
            {
                target.text = text[i] + target.text;
                yield return typeDelay;
            }
        }
    }
}
