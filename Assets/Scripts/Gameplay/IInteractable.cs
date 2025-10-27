using UnityEngine;

namespace HorrorGame.Gameplay
{
    /// <summary>
    /// Интерфейс для объектов, с которыми можно взаимодействовать
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Вызывается при взаимодействии с объектом
        /// </summary>
        void Interact();
    }
}
