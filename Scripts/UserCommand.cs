using UnityEngine;
using UnityEngine.Events;

namespace DialogueScript
{
    [System.Serializable]
    public class UserCommand
    {
        // –½—ß‚Ì–¼‘O‚ðÝ’è‚µ‚Ä‚¾‚³‚¢B
        [SerializeField]
        private string commandName;
        public string CommandName { get => commandName; set => commandName = value; }

        // –½—ß‚Ìˆ—‚ðÝ’è‚µ‚Ä‚­‚¾‚³‚¢B
        [SerializeField]
        private UnityEvent commandEvent;
        public UnityEvent CommandEvent { get => commandEvent; set => commandEvent = value; }
    }
}