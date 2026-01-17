using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace SoJoG
{
    [System.Serializable]
    public class InputActionBinding
    {
        [Header("Input Binding")]
        public InputActionReference actionReference;
        public UnityEvent onPerformed;

        [Header("Audio Feedback")]
        public AudioClip soundEffect;
        [Range(0, 1)] public float volume = 1f;
        public AudioSource audioSource;
    }

    public class InputActionPerformer : MonoBehaviour
    {
        [Header("Action Bindings")]
        [SerializeField] private InputActionBinding[] bindings = new InputActionBinding[0];

        // Track whether each binding has been performed
        private bool[] hasPerformed;
        private const float INPUT_COOLDOWN = 0.1f;

        private void Awake()
        {
            InitializePerformedStates();
        }

        private void OnEnable()
        {
            InitializePerformedStates();

            for (int i = 0; i < bindings.Length; i++)
            {
                int index = i;
                if (bindings[index].actionReference?.action != null)
                {
                    bindings[index].actionReference.action.performed += _ => HandleActionPerformed(index);
                    bindings[index].actionReference.action.Enable();
                }
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < bindings.Length; i++)
            {
                int index = i;
                if (bindings[index].actionReference?.action != null)
                {
                    bindings[index].actionReference.action.performed -= _ => HandleActionPerformed(index);
                    bindings[index].actionReference.action.Disable();
                }
            }
        }

        private void InitializePerformedStates()
        {
            // Reset all performed states when enabled
            hasPerformed = new bool[bindings.Length];
            for (int i = 0; i < hasPerformed.Length; i++)
            {
                hasPerformed[i] = false;
            }
        }

        private void HandleActionPerformed(int bindingIndex)
        {
            if (hasPerformed[bindingIndex])
                return;

            hasPerformed[bindingIndex] = true;

            var binding = bindings[bindingIndex];
            binding.onPerformed?.Invoke();

            if (binding.soundEffect != null)
            {
                var audioSourceToUse = binding.audioSource != null ?
                    binding.audioSource :
                    GetComponent<AudioSource>();

                if (audioSourceToUse != null)
                {
                    audioSourceToUse.PlayOneShot(binding.soundEffect, binding.volume);
                }
            }
        }
    }
}