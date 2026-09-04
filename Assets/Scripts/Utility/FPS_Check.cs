using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

public class FPS_Check : MonoBehaviour
{	
	float deltaTime = 0.0f;
	float worstFps = 100f;

	Color warning = new Color(255, 255, 0, 1.0f);
	Color danger = new Color(255, 0, 0, 1.0f);
	Color good = new Color(0, 255, 0, 1.0f);

	private GUIStyle style;

	private StringBuilder _stringBuilder = new StringBuilder();

	// Start is called before the first frame update
	void Start()
	{
		StartCoroutine("CheckReset");
	}

    IEnumerator CheckReset()
    {
	    while (true)
	    {
		    yield return new WaitForSeconds(15f);
		    worstFps = 100f;
	    }
    }

    // Update is called once per frame
    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		if (null == style)
		{
			style = new GUIStyle();
		}

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		if (fps < worstFps)
			worstFps = fps;

		if (fps > 40)
		{
			style.normal.textColor = good;
		}
		else if( fps > 20)
		{
			style.normal.textColor = warning;
		}
		else
		{
			style.normal.textColor = danger;
		}

		string text = "";

		text = string.Format("{0:0.0} ms ({1:0.} fps, {2:0.}, {3:0.})", msec, fps, worstFps,
			OnDemandRendering.effectiveRenderFrameRate);


		GUI.Label(rect, text, style);
	}
}
