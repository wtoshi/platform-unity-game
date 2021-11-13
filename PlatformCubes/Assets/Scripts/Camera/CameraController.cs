using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]    float duration;
    [SerializeField]    AnimationCurve animCurve;
    Camera myCam;

    private void Awake()
    {
        myCam = GetComponent<Camera>();
    }

    public void UpdatePosition(Vector3 _newPos)
    {
        StartCoroutine(doUpdatePosition(_newPos));
    }

    IEnumerator doUpdatePosition(Vector3 _newPos)
    {
        // Camera Position 
        Vector3 oldPosition = transform.position;
        _newPos.y = oldPosition.y;

        // Camera size
        float oldSize = GetComponent<Camera>().orthographicSize;
        float newSize = animCurve.Evaluate(GameManager.instance.LevelToSize);

        float currentTime = 0;

        Debug.Log("Size: "+ animCurve.Evaluate(GameManager.instance.LevelToSize));
        while (currentTime < duration)
        {
            transform.position = Vector3.Lerp(transform.position, _newPos, currentTime/duration);
            myCam.orthographicSize = Mathf.Lerp(oldSize, newSize, currentTime/duration);

            currentTime += Time.deltaTime;

            yield return null;
        }

        transform.position = _newPos;
        myCam.orthographicSize = newSize;
    }

    public void ChangeBGColor()
    {
        StartCoroutine(doChangeBGColor());
    }

    IEnumerator doChangeBGColor()
    {
        float duration = 4f;
        float currentStep = 0f;

        Color newColor = GetRandomColor();

        while(currentStep < duration)
        {
            myCam.backgroundColor = Color.Lerp(myCam.backgroundColor, newColor, currentStep / duration);
            currentStep += Time.deltaTime;
            yield return null;
        }

        myCam.backgroundColor = newColor;
    }

    Color GetRandomColor()								// Random Color
    {
        return new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
    }
}
