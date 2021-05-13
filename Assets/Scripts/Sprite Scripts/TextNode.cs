using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextNode : MonoBehaviour
{
	public float range = 5;
	[TextArea]
	public string text = "text";

	public TextMeshProUGUI stateDisplay;
	public Transform target;

    // Update is called once per frame
    void Update()
    {
		float targetDist = Vector3.Distance(target.position, transform.position);

		if (targetDist <= range)
		{
			stateDisplay.text = text;
			Color textCol = new Color(1, 1, 1, 1-(targetDist / range));
			stateDisplay.color = textCol;
		}
    }

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, range);
	}
}
