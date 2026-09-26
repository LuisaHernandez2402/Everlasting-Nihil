using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Anything that can participate in a chain reaction
    /// implements this interface.
    /// </summary>
    public interface IReactable
    {
        // Called whenever this object receives a reaction.
        void React();
    }
}