using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace AFramework.Promo
{
    [System.Serializable]
    public class PromoData
    {
        public string actionType;
        public string actionValue;
        public string width;
        public string height;
        public string videoUrl;
        public string imageUrl;
        public string promoUrl;

        public string VideoFileDownloaded;
        public string ImageFileDownloaded;

        Texture mCacheTexture = null;

        public bool IsValid() { return !string.IsNullOrEmpty(promoUrl) && (!string.IsNullOrEmpty(promoUrl) || !string.IsNullOrEmpty(promoUrl)); }

        public Texture GetImageTexture(bool cache)
        {
            if (mCacheTexture != null) return mCacheTexture;
            if (string.IsNullOrEmpty(ImageFileDownloaded)) return null;
            byte[] byteArray = System.IO.File.ReadAllBytes(ImageFileDownloaded);
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(byteArray);
            if (cache) mCacheTexture = texture;
            return texture;
        }

        public static bool operator == (PromoData left,
                                         PromoData right)
        {
            var leftNull = object.ReferenceEquals(left, null);
            var rightNull = object.ReferenceEquals(right, null);
            if (leftNull && rightNull) return true;
            else if (leftNull != rightNull) return false;

            return (
                (left.actionType == right.actionType)
                && (left.actionValue == right.actionValue)
                && (left.videoUrl == right.videoUrl)
                && (left.imageUrl == right.imageUrl)
                && (left.promoUrl == right.promoUrl)
                );
        }

        public static bool operator !=(PromoData left,
                                         PromoData right)
        {
            var leftNull = object.ReferenceEquals(left, null);
            var rightNull = object.ReferenceEquals(right, null);
            if (leftNull && rightNull) return false;
            else if (leftNull != rightNull) return true;

            return (
                (left.actionType != right.actionType)
                || (left.actionValue != right.actionValue)
                || (left.videoUrl != right.videoUrl)
                || (left.imageUrl != right.imageUrl)
                || (left.promoUrl != right.promoUrl)
                );
        }
    }

    public class VideoHandler
    {
        public Color BackgroundColor = Color.black;
        PromoData mPromoData;
        RawImage mVideoHolder;
        UnityEngine.Video.VideoPlayer mVideoPlayer;
        AudioSource mAudioSource;
        System.Action mEndCallback;
        RenderTexture mRenderTexture;

        public VideoHandler(RawImage videoHolder, PromoData promoData, System.Action endCallback, RenderTexture renderTexture = null)
        {
            mVideoHolder = videoHolder;
            mPromoData = promoData;
            mRenderTexture = renderTexture;
            mEndCallback = endCallback;

            mVideoPlayer = mVideoHolder.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
            if (mVideoPlayer == null) mVideoPlayer = mVideoHolder.gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();
            mVideoPlayer.loopPointReached += OnLoopPointReached;

            mAudioSource = mVideoHolder.gameObject.GetComponent<AudioSource>();
            if (mAudioSource == null) mAudioSource = mVideoHolder.gameObject.AddComponent<AudioSource>();

            mVideoPlayer.source = UnityEngine.Video.VideoSource.Url;
            mVideoPlayer.url = mPromoData.VideoFileDownloaded;
            mVideoPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.AudioSource;
            mVideoPlayer.EnableAudioTrack(0, true);
            mVideoPlayer.SetTargetAudioSource(0, mAudioSource);
            mVideoPlayer.Prepare();

            if (mRenderTexture == null)
            {
                mRenderTexture = new RenderTexture(int.Parse(mPromoData.width), int.Parse(mPromoData.height), 16, RenderTextureFormat.ARGB32);
            }
            mVideoPlayer.targetTexture = mRenderTexture;
            mVideoHolder.texture = mRenderTexture;
        }

        public void SetVolume(float volume)
        {
            mAudioSource.volume = volume;
        }

        public bool HasImage() { return !string.IsNullOrEmpty(mPromoData.ImageFileDownloaded); }

        public void ShowImage(bool cache)
        {
            mVideoHolder.texture = mPromoData.GetImageTexture(cache);
        }

        public void PlayVideo(bool loop)
        {
            APromoManager.I.StartCoroutine(CRPlayThread(loop));
        }

        IEnumerator CRPlayThread(bool loop)
        {
            while (!mVideoPlayer.isPrepared) yield return null;
            mVideoPlayer.isLooping = loop;
            mVideoPlayer.Play();
        }

        public bool IsPlaying() { return mVideoPlayer.isPlaying; }

        public void Close(bool freePlayer, bool freeAudio, bool freeRenderTexture)
        {
            mVideoPlayer.Stop();
            mAudioSource.Stop();

            if (freePlayer)
            {
                GameObject.Destroy(mVideoPlayer);
                mVideoPlayer = null;
            }

            if (freeAudio)
            { 
                GameObject.Destroy(mAudioSource);
                mAudioSource = null;
            }

            if (freeRenderTexture)
            {
                GameObject.Destroy(mRenderTexture);
                mRenderTexture = null;
            }

            mVideoHolder = null;
            mPromoData = null;
            mEndCallback = null;
        }

        void OnLoopPointReached(UnityEngine.Video.VideoPlayer source)
        {
            if (mEndCallback != null) mEndCallback();
        }
    }

    public class APromoManager : AFramework.SingletonMono<APromoManager>
    {
        public static System.Action EventOnDataChanged;
        public static System.Action EventOnDataDownloaded;

        const string PromoTempFile = "promodownload.temp";
        const string VideoFileExtension = "mp4";
        const string ImageFileExtension = "api";

        public PromoData Data = null;
        public virtual bool IsDownloading { get { return mDownloadThread != null; } }
        public float ErrorWaitTime = 3;
        //public float ShowWaitTime = 0.1f;

        protected IEnumerator mDownloadThread = null;
        protected UnityWebRequest mWebRequest = null;

        //protected override void Awake()
        //{
        //    base.Awake();//must keep this
        //}

        public virtual bool IsSafeToDownload() { return true; }
        public virtual bool IsSafeToShow() { return true; }
        public virtual bool IsDataReady()
        {
            return Data != null && !string.IsNullOrEmpty(Data.videoUrl) && !string.IsNullOrEmpty(Data.VideoFileDownloaded) && !string.IsNullOrEmpty(Data.imageUrl) && !string.IsNullOrEmpty(Data.ImageFileDownloaded);
        }

        public virtual bool OnReceivePromoData(string json)
        {
            PromoData newData = JsonUtility.FromJson<PromoData>(json);

            if (Data == newData) return false;

            Data = newData;
            if (mDownloadThread != null)
            {
                StopCoroutine(mDownloadThread);
            }
            mDownloadThread = CRDownload();
            StartCoroutine(mDownloadThread);
            if (EventOnDataChanged != null) EventOnDataChanged();
            return true;
        }

        protected IEnumerator CRDownload()
        {
            if (mWebRequest != null) yield return new WaitUntil(() => mWebRequest.isDone);
            mWebRequest = null;
            yield return new WaitUntil(() => IsSafeToDownload());

            string savepath = AFramework.Utility.GetSavePath();
            var tempFilePath = savepath + PromoTempFile;
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
                yield return null;
            }

            List<string> uniqueExtensionList = new List<string>();
            uniqueExtensionList.Add(VideoFileExtension);
            uniqueExtensionList.Add(ImageFileExtension);

            //simple check first, will write correctly if data array is used
            List<string> nameList = new List<string>();
            List<string> extensionList = new List<string>();
            List<string> urlList = new List<string>();
            {
                var fileName = Utility.GetUrlFilename(Data.videoUrl);
                if (fileName != null)
                {
                    nameList.Add(fileName.Remove(fileName.IndexOf('.')));
                    extensionList.Add(VideoFileExtension);
                    urlList.Add(Data.videoUrl);
                }
            }
            {
                var fileName = Utility.GetUrlFilename(Data.imageUrl);
                if (fileName != null)
                {
                    nameList.Add(fileName.Remove(fileName.IndexOf('.')));
                    extensionList.Add(ImageFileExtension);
                    urlList.Add(Data.imageUrl);
                }
            }

            //search and delete old files
            for (int i = 0; i < uniqueExtensionList.Count; ++i)
            {
                string[] files = System.IO.Directory.GetFiles(savepath, "*." + uniqueExtensionList[i]);
                yield return null;
                if (files == null || files.Length <= 0) continue;
                
                for (int j = 0; j < files.Length; ++j)
                {
                    bool fileNeeded = false;
                    int fileNameHash = -1;
                    int.TryParse(Path.GetFileNameWithoutExtension(files[j]), out fileNameHash);
                    for (int k = 0; k < nameList.Count; ++k)
                    {
                        if (extensionList[k] != uniqueExtensionList[i]) continue;
                        if (nameList[k].GetHashCode() == fileNameHash)
                        {
                            fileNeeded = true;
                            break;
                        }
                    }

                    if (!fileNeeded)
                    {
                        File.Delete(files[j]);
                        yield return null;
                    }
                }
            }

            //download file
            var waitSafeCondition = new WaitUntil(() => IsSafeToDownload() && Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
            var waitError = new WaitForSeconds(ErrorWaitTime);
            for (int i = 0; i < nameList.Count; ++i)
            {
                yield return waitSafeCondition;
                var saveFilePath = savepath + nameList[i].GetHashCode() + "." + extensionList[i];
                if (File.Exists(saveFilePath))
                {
                    continue;
                }

                mWebRequest = UnityWebRequest.Get(urlList[i]);
                mWebRequest.method = UnityWebRequest.kHttpVerbGET;
                DownloadHandlerFile dlh = new DownloadHandlerFile(tempFilePath);
                mWebRequest.downloadHandler = dlh;
                dlh.removeFileOnAbort = true;
                yield return mWebRequest.SendWebRequest();
                while (!mWebRequest.isDone)
                {
                    yield return null;
                }

                if (mWebRequest.isHttpError || mWebRequest.isNetworkError)
                {
                    mWebRequest = null;
                    yield return waitError;
                }
                else
                {
                    mWebRequest = null;
                    yield return null;//wait a frame to make sure file is ready;
                    if (File.Exists(tempFilePath))
                    {
                        File.Move(tempFilePath, saveFilePath);
                        yield return null;
                    }
                }
            }

            mDownloadThread = null;

            Data.VideoFileDownloaded = savepath + nameList[0].GetHashCode() + "." + extensionList[0];
            if (nameList.Count > 1)
            {
                Data.ImageFileDownloaded = savepath + nameList[1].GetHashCode() + "." + extensionList[1];
            }
            if (EventOnDataDownloaded != null) EventOnDataDownloaded();
        }

        public VideoHandler PlayVideo(RawImage videoHolder, System.Action endCallback)
        {
            return new VideoHandler(videoHolder, Data, endCallback);
        }

        public void PlayVideoFullscreen(System.Action callback, float safeStateWaitTime = 0f)
        {
            if (Data == null || Data.VideoFileDownloaded == null)
            {
                Debug.LogWarning(Data == null ? "No crosspromo data" : "CrossPromoVideo is not downloaded");
                return;
            }
            var backupCallback = callback;
            StartCoroutine(SimpleCRPlayPromo(backupCallback, safeStateWaitTime));
        }

        IEnumerator SimpleCRPlayPromo(System.Action callback, float safeStateWaitTime = 0f)
        {
            var backupCallback = callback;
            if (safeStateWaitTime > 0)
            {
                while (!IsSafeToShow())
                {
                    yield return null;
                    safeStateWaitTime -= Time.deltaTime;
                }
            }
            if (!IsSafeToShow())
            {
                Debug.LogWarning("Current state does not safe to show cross promo");
                yield break;
            }
            AFramework.UI.CanvasManager.ShowSystemLoadingPopup(true);
            yield return null;
#if UNITY_IOS || UNITY_ANDROID
            Handheld.PlayFullScreenMovie(AFramework.Promo.APromoManager.I.Data.videoUrl, Color.black, FullScreenMovieControlMode.CancelOnInput);
#endif
            yield return null;
            AFramework.UI.CanvasManager.ShowSystemLoadingPopup(false);
            if (backupCallback != null) backupCallback();
        }
    }
}