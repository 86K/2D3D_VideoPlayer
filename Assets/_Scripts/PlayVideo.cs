/*
*R0-V1.0
*Modify Date:2018-10-23
*Modifier:ZoJet
*Modify Reason:播放视频
*Modify Content:
*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    #region -- 变量定义
    public VideoPlayer videoPlayer;//VideoPlayer组件

    public Slider timeSlider;//播放进度条
    public Slider playSpeedSlider;//播放速度进度条
    public Slider volumeSlider;//音量进度条

    private static string totalTime = "";//总时间
    private static string currentTime = "";//当前时间
    public Text showTime;//显示时间进度

    private Sprite[] sprites;//存放所有的缩略图
    private string[] spriteNames;//存放所有缩略图的名字
    private GameObject[] videoItems;//所有缩略图按钮
    public Transform content;//管理所有缩略图按钮
    public GameObject videoPanel;//视频面板

    private string[] videoFilesPath;//视频文件路径
    private string[] videoNames;//视频文件名称
    private int videoIndex = 0;//视频文件组下标

    public AudioSource audioSource;//声音播放源
    public Button playOrStop;//播放暂停按钮
    #endregion

    #region -- 系统函数
    void Awake()
    {
        Init();
    }

    void Start()
    {
        //取消自动播放
        videoPlayer.playOnAwake = false;
        //默认播放xml里第一个视频
        videoPlayer.url = videoFilesPath[0];

        //默认播放速度为正常的1倍速 范围[0,2]
        playSpeedSlider.value = 1;
        //默认音量为正常的1 范围[0,1]
        volumeSlider.value = 1;
    }

    void Update()
    {
        //播放速率跟随对应滑动条数值
        videoPlayer.playbackSpeed = playSpeedSlider.value;
        //音量大小跟随对应滑动条数值
        audioSource.volume = volumeSlider.value;

        TimeFollowVideo();
        SetPlayOrPauseButtonIcon();

        if (videoPanel.activeSelf) {
            videoPlayer.Pause();
        }

        Debug.Log(playOrStop.image.sprite.name);
    }
    #endregion

    #region -- 自定义函数
    /// <summary>
    /// 初始化模式  获取视频文件路径
    /// </summary>
    private void Init()
    {
        string xmlPath = Application.streamingAssetsPath + "/Config.xml";
        string[] datas = Xml.ReadElements(xmlPath, new string[] { "configuration", "videoplayer", "parameters", "VideoNames" });

        videoNames = new string[datas.Length];
        videoFilesPath = new string[datas.Length];
        for (int i = 0; i < datas.Length; i++)
        {
            //存储所有视频名称
            videoNames[i] = datas[i].Split('.')[0];
            //读取所有视频路径
            videoFilesPath[i] = Application.streamingAssetsPath + "\\" + "Videos" + "\\" + datas[i];
        }

        //获取所有图片并存储为sprite 
        string imagePath = Application.streamingAssetsPath + "/Images";
        sprites = Picture.GetSprites(imagePath, "*.png", 1920, 1080);
        
        videoItems = new GameObject[sprites.Length];
        spriteNames = new string[sprites.Length];
        for (int i = 0; i < sprites.Length; i++) {
            //实例化所有可选视频缩略图
            videoItems[i] = Instantiate((GameObject)Resources.Load("VideoItem"));
            videoItems[i].GetComponent<Image>().sprite = sprites[i];
            videoItems[i].transform.SetParent(content);
            //存储所有图片名
            spriteNames[i] = sprites[i].name;

            Button button = videoItems[i].GetComponent<Button>();
            button.onClick.AddListener(delegate {
                string name = button.GetComponent<Image>().sprite.name;
                for (int j = 0; j < videoNames.Length; j++) {
                    if (name == videoNames[j]) {
                        if(videoIndex != j) {
                            SwitchTheVideo();
                            videoIndex = j;
                            videoPlayer.url = videoFilesPath[videoIndex];
                        }
                        break;
                    }
                }
                //隐藏面板
                videoPanel.SetActive(false);
            });
        }
    }

    /// <summary>
    /// 设置播放或暂停按钮的图标
    /// </summary>
    private void SetPlayOrPauseButtonIcon()
    {
        if (videoPlayer.isPlaying)
        {
            playOrStop.image.sprite = playOrStop.spriteState.pressedSprite;
            return;
        }
        else
        {
            playOrStop.image.sprite = playOrStop.spriteState.disabledSprite;
        }
    }

    /// <summary>
    /// 播放或暂停视频
    /// </summary>
    public void PlayOrPauseVideoOnClick()
    {
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
        }
        else if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
    }

    /// <summary>
    /// 切换视频
    /// </summary>
    private void SwitchTheVideo() {
        playOrStop.image.sprite = playOrStop.spriteState.disabledSprite;
        currentTime = "";
        totalTime = "";
        timeSlider.value = 0;
        showTime.text = "";
    }

    /// <summary>
    /// 播放上一个视频
    /// </summary>
    /// <param name="button"></param>
    public void PlayLastVideoOnClick()
    {
        SwitchTheVideo();

        videoIndex--;
        if (videoIndex == -1)
        {
            videoIndex = videoFilesPath.Length - 1;
        }
        videoPlayer.url = videoFilesPath[videoIndex];
    }

    /// <summary>
    /// 播放下一个视频
    /// </summary>
    public void PlayNextVideoOnClick()
    {
        SwitchTheVideo();

        videoIndex++;
        if (videoIndex == videoFilesPath.Length)
        {
            videoIndex = 0;
        }
        videoPlayer.url = videoFilesPath[videoIndex];
    }

    /// <summary>
    /// 滑动条、时间显示跟随视频时间
    /// </summary>
    private void TimeFollowVideo()
    {
        //进度条跟随播放时间
        if (videoPlayer.isPlaying)
        {
            timeSlider.value = (float)(videoPlayer.frame) / (videoPlayer.frameCount);

            //得到视频总时间
            if (totalTime == "")
            {
                totalTime = (videoPlayer.frameCount / videoPlayer.frameRate).ToString("f1");
            }

            //得到当前时间并显示当前时间/总时间
            currentTime = (videoPlayer.frame / videoPlayer.frameRate).ToString("f1");
            showTime.text = currentTime + " / " + totalTime;

            if (float.Parse(currentTime) >= float.Parse(totalTime))
            {
                timeSlider.value = 1;
                showTime.text = totalTime + " / " + totalTime;
            }
        }
    }

    /// <summary>
    /// 点击隐藏或显示物体
    /// </summary>
    public void HideOrDisplayGameObjectOnClick(GameObject go)
    {
        if (go.activeSelf)
        {
            go.SetActive(false);
        }
        else
        {
            go.SetActive(true);
        }
    }
    #endregion
}
