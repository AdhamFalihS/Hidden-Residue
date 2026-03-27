using UnityEngine;

namespace HiddenResidue.Dialog
{
    [System.Serializable]
    public class DialogLine
    {
        public string speakerName = "Jojo";

        [TextArea(2, 5)]
        public string text = "Aku harus membersihkan tempat ini.";
        public Sprite portrait;

    }

    [CreateAssetMenu(fileName = "DialogData_", menuName = "HiddenResidue/Dialog Data")]

    public class DialogData : ScriptableObject

    {

        public DialogLine[] lines;

    }

}