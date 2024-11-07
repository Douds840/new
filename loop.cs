using UnityEngine;
using UnityEngine.Video;

public class loop : MonoBehaviour
{
    [SerializeField] private VideoClip videoClip;     // The video clip you want to loop
    [SerializeField] private RenderTexture renderTexture; // The RenderTexture to display the video
    private VideoPlayer videoPlayer;                  // Reference to the VideoPlayer component

    void Start()
    {
        // Get the VideoPlayer component attached to this GameObject
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            // If no VideoPlayer is attached, add one
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }

        // Set the video clip to the VideoPlayer
        videoPlayer.clip = videoClip;

        // Set the RenderTexture as the target texture
        videoPlayer.targetTexture = renderTexture;

        // Set the VideoPlayer to loop
        videoPlayer.isLooping = true;

        // Play the video
        videoPlayer.Play();
    }
}
