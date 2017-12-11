using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {
    public Animator animator;
    public Text damageFeedback;

	void Start () {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
	}

    public void setTextoDeFeedback(string texto){
        damageFeedback.text = texto;
    }
}
