namespace GestionaireEmployes.Models
{
    public class ChatMessage
    {
        public int Id { get; set; } // Clé primaire
        public string User { get; set; } // Nom de l'utilisateur
        public string Message { get; set; } // Contenu du message
        public DateTime Timestamp { get; set; } // Horodatage du message
    }
}
