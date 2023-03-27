using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShipDock
{

    public class LanguageComponent : MonoBehaviour
    {

        [SerializeField]
        private string[] m_LanguageIDs;
        [SerializeField]
        private Text[] m_Labels;
        [SerializeField]
        private TextMesh[] m_LabelMeshs;

        private void Awake()
        {
            ShipDockApp shipDockApp = ShipDockApp.Instance;
            if (shipDockApp != default && m_Labels != default)
            {
                Text ui;
                TextMesh textMesh;
                string id, content;
                int max = m_LanguageIDs.Length;
                for (int i = 0; i < max; i++)
                {
                    id = m_LanguageIDs[i];

                    ui = i < m_Labels.Length ? m_Labels[i] : default;
                    textMesh = i < m_LabelMeshs.Length ? m_LabelMeshs[i] : default;

                    content = shipDockApp.Locals.Language(id);

                    if (!string.IsNullOrEmpty(id) && ui != default)
                    {
                        ui.text = content;
                    }
                    else { }

                    if (!string.IsNullOrEmpty(id) && textMesh != default)
                    {
                        textMesh.text = content;
                    }
                    else { }

                    SetTextLanguage(i, ref content);
                }
            }
            else { }
        }

        protected virtual void SetTextLanguage(int index, ref string content) { }

        public void AddLanguageLabel(Text text, string languageID)
        {
            List<string> ids = new List<string>(m_LanguageIDs);
            List<Text> list = new List<Text>(m_Labels);

            ids.Add(languageID);
            list.Add(text);

            m_LanguageIDs = ids.ToArray();
            m_Labels = list.ToArray();
        }

        public void AddLanguageLabelMesh(TextMesh text, string languageID)
        {
            List<string> ids = new List<string>(m_LanguageIDs);
            List<TextMesh> list = new List<TextMesh>(m_LabelMeshs);

            ids.Add(languageID);
            list.Add(text);

            m_LanguageIDs = ids.ToArray();
            m_LabelMeshs = list.ToArray();
        }
    }

}