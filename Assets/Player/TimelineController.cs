using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimelineController : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject timeline;
    public GameObject trackPrefab;
    public GameObject linePrefab;
    public GameObject textPrefab;
    public GameObject timelineCursor;

    public Canvas timelineLabelCanvas;

    private Vector3 _initialTimelinePosition;

    class Channel {
        public CreationElement.Type type;
        public List<CreationElement> creationElements;
        public List<GameObject> creationElementBlocks;
        public bool isSelected;

        public List<CreationElement> creationElementsToAdd;

        public Channel(CreationElement.Type type, bool isSelected) {
            this.type = type;
            this.creationElements = new List<CreationElement>();
            this.creationElementsToAdd = new List<CreationElement>();
            this.creationElementBlocks = new List<GameObject>();
            this.isSelected = isSelected;
        }
    }

    private List<Channel> _channels = new List<Channel>();


    private List<GameObject> _channelTrackGameObjects = new List<GameObject>();
    private List<GameObject> _channelTrackLineGameObjects = null;
    private List<GameObject> _timelineLabelGameObjects = null;
    private GameObject _waveformTrackGameObject;


    private int _selectedIndex = 0;

    private static Color[] COLORS = {
        Color.blue,
        Color.clear,
        Color.cyan,
        Color.gray,
        Color.green,
        Color.grey,
        Color.magenta,
        Color.red,
        Color.yellow
    };

    System.Random random = new System.Random(); 

    // Start is called before the first frame update
    void Start()
    {
        _initialTimelinePosition = new Vector3(0, 1, 0);
        _channels.Add(new Channel(CreationElement.Type.SFX, true));
        _channels.Add(new Channel(CreationElement.Type.VFX, false));
        _channels.Add(new Channel(CreationElement.Type.Skybox, false));
    }

    public void AddCreationElement(CreationElement element) {
        if (element.type == CreationElement.Type.SFX) {
            Channel sfxChannel = _channels[0];
            sfxChannel.creationElementsToAdd.Add(element);
        }

        if (element.type == CreationElement.Type.VFX) {
            Channel vfxChannel = _channels[1];
            vfxChannel.creationElementsToAdd.Add(element);
        }

        if (element.type == CreationElement.Type.Skybox) {
            Channel skyboxChannel = _channels[2];
            skyboxChannel.creationElementsToAdd.Add(element);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("left")) {
            _channels[_selectedIndex].isSelected = false;
            _selectedIndex = (_selectedIndex - 1 + _channels.Count) % _channels.Count;
            _channels[_selectedIndex].isSelected = true;
        }

        if (Input.GetKeyDown("right")) {
            _channels[_selectedIndex].isSelected = false;
            _selectedIndex = (_selectedIndex + 1) % _channels.Count;
            _channels[_selectedIndex].isSelected = true;
        }
        

        // Moving the timeline
        timeline.transform.position = -timeline.transform.forward * audioSource.time + _initialTimelinePosition;

        int numTracks = _channels.Count + 1;

        float gap = 0.125f;
        float trackWidth = trackPrefab.transform.localScale.x;
        float timelineWidth = numTracks * trackWidth + Mathf.Max(0, numTracks - 1) * gap;

        // Adjust the size of the timeline cursor
        Vector3 timelineCursorScale = timelineCursor.transform.localScale;
        timelineCursor.transform.localScale = new Vector3(timelineWidth, timelineCursorScale.y, timelineCursorScale.z);

        float timelineStartingPosX = -timelineWidth/2 + trackWidth / 2;

        // Position Waveform Track
        if (_waveformTrackGameObject == null) {
            _waveformTrackGameObject = CreateTrackBlock();

            // Set the waveform on the selected channel
            Renderer renderer = _waveformTrackGameObject.GetComponent<Renderer>();
            renderer.material.mainTexture = GetOrCreateWaveformTexture();
        }

        Vector3 waveformTrackPosition = _waveformTrackGameObject.transform.localPosition;
        _waveformTrackGameObject.transform.localPosition = new Vector3(timelineStartingPosX, waveformTrackPosition.y, waveformTrackPosition.z);

        // Position the channel tracks
        for (int i = 0; i < _channels.Count; i += 1) {
            Channel channel = _channels[i];
            
            // Get or create channels
            GameObject channelTrackGameObject = GetOrCreateNthChannelTrackBlock(channel, i);

            // Re-position the channel
            Vector3 channelTrackLocalPos = channelTrackGameObject.transform.localPosition;
            float channelPosX = timelineStartingPosX + (i + 1) * (trackWidth + gap);
            channelTrackGameObject.transform.localPosition = new Vector3(channelPosX, channelTrackLocalPos.y, channelTrackLocalPos.z);
        }

        int numDemarcations = Mathf.CeilToInt(audioSource.clip.length);

        // Segment the channel tracks into seconds, by adding lines
        if (_channelTrackLineGameObjects == null) {
            _channelTrackLineGameObjects = new List<GameObject>();

            float numChannels = _channels.Count;
            float channelContainerWidth = numChannels * trackWidth + Mathf.Max(0, numChannels - 1) * gap;
            float channelStartPosX = channelContainerWidth / 2 - (timelineWidth / 2 - (trackWidth + gap));

            for (int i = 0; i < numDemarcations; i += 1) {
                GameObject line = Instantiate(linePrefab);

                Vector3 linePosition = line.transform.localPosition;
                line.transform.localPosition = new Vector3(channelStartPosX, linePosition.y, i);

                Vector3 lineScale = line.transform.localScale;
                line.transform.localScale = new Vector3(channelContainerWidth, lineScale.y, lineScale.z);

                _channelTrackLineGameObjects.Add(line);
                line.transform.SetParent(timeline.transform);
            }
        }

        // Add the timeline labels
        if (_timelineLabelGameObjects == null) {
            _timelineLabelGameObjects = new List<GameObject>();

            float textWidth = textPrefab.transform.localScale.x;
            float textStartinPosX = -timelineWidth/2 - (textWidth / 2 + gap);

            for(int i = 0; i < numDemarcations; i++) {
                GameObject labelGameObject = Instantiate(textPrefab, new Vector3(textStartinPosX, 0, i), Quaternion.Euler(90, 0, 0));
                labelGameObject.GetComponent<TextMeshProUGUI>().text = i.ToString();
                labelGameObject.transform.SetParent(timelineLabelCanvas.transform);
                _timelineLabelGameObjects.Add(labelGameObject);

                for (float i2 = i + 0.25f; i2 < i + 1; i2 += 0.25f){
                    labelGameObject = Instantiate(textPrefab, new Vector3(textStartinPosX, 0, i2), Quaternion.Euler(90, 0, 0));
                    labelGameObject.GetComponent<TextMeshProUGUI>().text = "-";
                    labelGameObject.transform.SetParent(timelineLabelCanvas.transform);
                    _timelineLabelGameObjects.Add(labelGameObject);
                }
            }
        }

        // Render new blocks onto the timeline
        for (int channelI = 0; channelI < _channels.Count; channelI += 1) {
            Channel channel = _channels[channelI];

            // Paige's suggestion to make block smaller
            // TODO: Paige's other suggestion: make the blocks narrower
            float trackHeight = trackPrefab.transform.localScale.y * 0.75f;

            // Place blocks on timeline, if necessary
            for (int j = 0; j < channel.creationElementsToAdd.Count; j += 1) {
                CreationElement creationElement = channel.creationElementsToAdd[j];

                GameObject creationElementBlock = Instantiate(trackPrefab);
                Color randomColor = COLORS[random.Next(0, COLORS.Length)];

                creationElementBlock.GetComponent<Renderer>().material.color = randomColor;
                creationElementBlock.transform.SetParent(timeline.transform);

                // Calculate Block Position Z
                Vector3 creationElementBlockPos = creationElementBlock.transform.localPosition;
                float creationBlockPosY = creationElementBlockPos.y + trackHeight + 0.05f;

                for (int k = 0; k < channel.creationElements.Count; k += 1) {
                    float previousStartTime = channel.creationElements[k].startTime;
                    float previousEndTime = channel.creationElements[k].endTime;

                    if (creationElement.startTime <= previousEndTime && creationElement.endTime >= previousStartTime) {
                        creationBlockPosY = channel.creationElementBlocks[k].transform.localPosition.y + trackHeight + 0.02f;
                    }
                }

                // Calculate Block Position X
                float channelPosX = timelineStartingPosX + (channelI + 1) * (trackWidth + gap);
                float blockLength = creationElement.endTime - creationElement.startTime;

                // Set Block position
                creationElementBlock.transform.localPosition = new Vector3(channelPosX, creationBlockPosY, creationElement.startTime + blockLength / 2);

                // Set Block size
                Vector3 creationElementBlockSize = creationElementBlock.transform.localScale;
                creationElementBlock.transform.localScale = new Vector3(creationElementBlockSize.x, creationElementBlockSize.y, blockLength);

                channel.creationElements.Add(creationElement);
                channel.creationElementBlocks.Add(creationElementBlock);
            }

            channel.creationElementsToAdd.Clear();

        }
    }

    private GameObject GetOrCreateNthChannelTrackBlock(Channel channel, int i) {
        if (i < _channelTrackGameObjects.Count) {
            return _channelTrackGameObjects[i];
        }

        GameObject channelTrackGameObject = CreateTrackBlock();
        _channelTrackGameObjects.Add(channelTrackGameObject);
        return channelTrackGameObject;
    }

    private GameObject CreateTrackBlock() {
        GameObject trackBlock = Instantiate(trackPrefab);
        float audioLength = audioSource.clip.length;

        Vector3 trackLocalScale = trackBlock.transform.localScale;
        trackBlock.transform.localScale = new Vector3(trackLocalScale.x, trackLocalScale.y, audioLength);

        Vector3 trackLocalPosition = trackBlock.transform.localPosition;
        trackBlock.transform.localPosition = new Vector3(trackLocalPosition.x, 0, audioLength/2.0f);
        trackBlock.transform.SetParent(timeline.transform);

        return trackBlock;
    }

    Texture2D _waveFormTexture = null;

    private Texture2D GetOrCreateWaveformTexture() {
        if (_waveFormTexture == null) {
            int audioLength = Mathf.CeilToInt(audioSource.clip.length);
            int audioAmplitude = 2;

            int pixelLength = 1920 * 4;
            int pixelWidth = Mathf.CeilToInt(audioAmplitude * (pixelLength * 1.0f) / audioLength);

            _waveFormTexture = PaintWaveformSpectrum(audioSource.clip, pixelLength, pixelWidth, Color.black);
            return _waveFormTexture;
        }
        return _waveFormTexture;
    }


    private static Texture2D PaintWaveformSpectrum(AudioClip audio, int length, int width, Color col) {
        Texture2D tex = new Texture2D(width, length, TextureFormat.RGBA32, false);
        float[] samples = new float[audio.samples];
        float[] waveform = new float[length];

        audio.GetData(samples, 0);

        int packSize = ( audio.samples / length ) + 1;

        int s = 0;
        for (int i = 0; i < audio.samples; i += packSize) {
            waveform[s] = Mathf.Abs(samples[i]);
            s++;
        }

        // Set background
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                tex.SetPixel(x, y, Color.white);
            }
        }

        // Set Waveform
        for (int y = 0; y < waveform.Length; y++) {
            for (int x = 0; x <= waveform[y] * ((float)width * .75f); x++) {
                // y = 0 is vertically center of image
                tex.SetPixel(( width / 2 ) + x, y, col);
                // symmetrical about y = 0
                tex.SetPixel(( width / 2 ) - x, y, col);
            }
        }

        tex.Apply();

        return tex;
    }

    public void EnterPreviewMode() {
        timeline.SetActive(false);
        timelineCursor.SetActive(false);
        //timelineLabelCanvas.SetActive(false);

    }

    public void ExitPreviewMode() {
        timeline.SetActive(true);
        timelineCursor.SetActive(true);
        //timelineLabelCanvas.SetActive(false);

    }
}
