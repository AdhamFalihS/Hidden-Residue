namespace HiddenResidue.Interaction
{
    /// <summary>
    /// IInteractable — Interface yang harus diimplementasi oleh semua
    /// objek yang bisa diinteraksi pemain dengan tombol E.
    ///
    /// Implementasi:
    ///   - CleanableObject.cs
    ///   - EvidenceObject.cs
    ///   - LockedDoor.cs
    ///   - InspectableObject.cs
    /// </summary>
    public interface IInteractable
    {
        /// <summary>Dipanggil saat pemain menekan E pada objek ini.</summary>
        void Interact();

        /// <summary>Apakah objek ini saat ini bisa diinteraksi?</summary>
        bool CanInteract { get; }

        /// <summary>
        /// Teks yang ditampilkan di indikator (default: "Tekan E").
        /// Bisa di-override per objek, misal "Tekan E untuk Bersihkan".
        /// </summary>
        string InteractPrompt { get; }
    }
}
