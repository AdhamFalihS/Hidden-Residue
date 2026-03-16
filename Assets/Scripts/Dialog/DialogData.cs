using UnityEngine;

namespace HiddenResidue.Dialog
{
    /// <summary>
    /// DialogLine — Satu baris dialog.
    /// </summary>
    [System.Serializable]
    public class DialogLine
    {
        public string  speakerName  = "Jojo";
        [TextArea(2, 5)]
        public string  text         = "Aku harus membersihkan tempat ini.";
        public Sprite  portrait;          // Foto karakter (opsional)
    }

    /// <summary>
    /// DialogData — ScriptableObject satu sesi dialog Visual Novel.
    /// Buat via: klik kanan → Create → HiddenResidue → Dialog Data
    /// </summary>
    [CreateAssetMenu(fileName = "DialogData_", menuName = "HiddenResidue/Dialog Data")]
    public class DialogData : ScriptableObject
    {
        public DialogLine[] lines;
    }
}
