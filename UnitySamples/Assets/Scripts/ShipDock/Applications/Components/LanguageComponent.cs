using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShipDock.Applications {

    public class LanguageComponent : MonoBehaviour {

        [SerializeField]
        private string[] m_LanguageIDs;
        [SerializeField]
        private Text[] m_Labels;

        private void Awake() {
            ShipDockApp shipDockApp = ShipDockApp.Instance;
            if (shipDockApp != default && m_Labels != default) {

                Text ui;
                string id;
                int max = m_LanguageIDs.Length;
                for (int i = 0; i < max; i++) {

                    id = m_LanguageIDs[i];
                    ui = m_Labels[i];

                    if (!string.IsNullOrEmpty(id) && ui != default) {
                        ui.text = shipDockApp.Locals.Language(id);
                    } else { }
                }
            } else { }
        }

        public void AddLanguageLabel(Text text, string languageID)
        {
            List<string> ids = new List<string>(m_LanguageIDs);
            List<Text> list = new List<Text>(m_Labels);

            ids.Add(languageID);
            list.Add(text);

            m_LanguageIDs = ids.ToArray();
            m_Labels = list.ToArray();
        }
    }

}