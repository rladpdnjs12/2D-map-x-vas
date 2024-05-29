using UnityEngine;

namespace etc
{
    public class GameSettingsManager : MonoBehaviour
    {
        public int targetFrameRate = 60;

        void Start()
        {
            // 프레임 속도 설정
            Application.targetFrameRate = targetFrameRate;

            // V-Sync 설정 해제
            QualitySettings.vSyncCount = 0;

            // 렌더링 설정
            SetRenderingSettings();
        }

        void SetRenderingSettings()
        {
            // 안티앨리어싱 설정 (0 = 없음, 2 = 2x, 4 = 4x, 8 = 8x)
            QualitySettings.antiAliasing = 4;

            // 그림자 품질 설정 (ShadowResolution.Low, ShadowResolution.Medium, ShadowResolution.High, ShadowResolution.VeryHigh)
            QualitySettings.shadowResolution = ShadowResolution.High;

            // 텍스처 품질 설정 (0 = Full Res, 1 = Half Res, 2 = Quarter Res, 3 = Eighth Res)
            QualitySettings.globalTextureMipmapLimit = 1;

            // 최대 LOD 레벨 설정
            QualitySettings.maximumLODLevel = 1;
        }
    }
}
