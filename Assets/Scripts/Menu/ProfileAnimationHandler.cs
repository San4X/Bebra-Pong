using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class ProfileAnimationHandler : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Button profileBtn, colorBtn, modeBtn;
        [SerializeField] private TMP_InputField inputField;

        private bool _isOutAnim = true;
    
        private void Start()
        {
            profileBtn.onClick.AddListener(PlayAnimation);
            modeBtn.onClick.AddListener(() =>
            {
                CloseProfile();
                profileBtn.interactable = false;
            });
            colorBtn.onClick.AddListener(() =>
            {
                CloseProfile();
                profileBtn.interactable = false;
            });
            
            inputField.onEndEdit.AddListener((arg0) =>
            {
                CloseProfile();
            });

            inputField.text = "Player" + Random.Range(1234, 4321);
        }
        private void PlayAnimation()
        {
            animator.SetTrigger(_isOutAnim ? "PlayOut" : "PlayIn");
            _isOutAnim = !_isOutAnim;
        }

        private void CloseProfile()
        {
            if (!_isOutAnim) PlayAnimation();
        }
    }
}
