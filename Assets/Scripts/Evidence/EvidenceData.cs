using UnityEngine;

namespace HiddenResidue.Evidence

{

    [CreateAssetMenu(fileName = "EvidenceData_", menuName = "HiddenResidue/Evidence Data")]

    public class EvidenceData : ScriptableObject

    {

        [Header("Info")]

        public string evidenceName        = "Kartu Identitas";

        [TextArea(2, 4)]

        public string description         = "Kartu identitas korban yang tertinggal di bawah meja.";

        public Sprite icon;

        [Header("ID")]

        [Tooltip("ID unik untuk evidence ini, dipakai sebagai key di save system")]

        public string evidenceID          = "evidence_001";

    }

}