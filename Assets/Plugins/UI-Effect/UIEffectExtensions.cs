using DG.Tweening;
using UnityEngine;

namespace Coffee.UIEffects
{
    public static class UIEffectExtensions
    {
        #region Tone
        public static Tween DOToneIntensity(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.toneIntensity, x => effect.toneIntensity = x, target, duration).SetTarget(effect);
        }
        #endregion

        #region Color
        public static Tween DOColorIntensity(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.colorIntensity, x => effect.colorIntensity = x, target, duration).SetTarget(effect);
        }

        public static Tween DOColorHueShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.colorHueShift, x => effect.colorHueShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOColorSaturationShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.colorSaturationShift, x => effect.colorSaturationShift    = x, target, duration).SetTarget(effect);
        }

        public static Tween DOColorValueShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.colorValueShift, x => effect.colorValueShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOColorContrastShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.colorContrastShift, x => effect.colorContrastShift    = x, target, duration).SetTarget(effect);
        }

        public static Tween DOColorBrightnessShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.colorBrightnessShift, x => effect.colorBrightnessShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOColorAlpha(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.colorAlpha, x => effect.colorAlpha = x, target, duration).SetTarget(effect);
        }
        #endregion

        #region Sampling
        public static Tween DOSamplingIntensity(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.samplingIntensity, x => effect.samplingIntensity = x, target, duration).SetTarget(effect);
        }

        public static Tween DOSamplingWidth(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.samplingWidth, x => effect.samplingWidth = x, target, duration).SetTarget(effect);
        }

        public static Tween DOSamplingScale(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.samplingScale, x => effect.samplingScale = x, target, duration).SetTarget(effect);
        }
        #endregion

        #region Transition
        public static Tween DOTransitionRate(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.transitionRate, x => effect.transitionRate = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionTextureScale(this UIEffect effect, Vector2 target, float duration)
        {
            return DOTween.To(() => effect.transitionTextureScale, x => effect.transitionTextureScale = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionTextureOffset(this UIEffect effect, Vector2 target, float duration)
        {
            return DOTween.To(() => effect.transitionTextureOffset, x => effect.transitionTextureOffset = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionTextureSpeed(this UIEffect effect, Vector2 target, float duration)
        {
            return DOTween.To(() => effect.transitionTextureSpeed, x => effect.transitionTextureSpeed = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionRotation(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.transitionRotation, x => effect.transitionRotation = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionWidth(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.transitionWidth, x => effect.transitionWidth = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionSoftness(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.transitionSoftness, x => effect.transitionSoftness = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionColor(this UIEffect effect, Color target, float duration)
        {
            return DOTween.To(() => effect.transitionColor, x => effect.transitionColor = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionColorHueShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.transitionColorHueShift, x => effect.transitionColorHueShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionColorSaturationShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.transitionColorSaturationShift, x => effect.transitionColorSaturationShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionColorValueShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.transitionColorValueShift, x => effect.transitionColorValueShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionColorContrastShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.transitionColorContrastShift, x => effect.transitionColorContrastShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionColorBrightnessShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.transitionColorBrightnessShift, x => effect.transitionColorBrightnessShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionColorAlpha(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.transitionColorAlpha, x => effect.transitionColorAlpha = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTransitionAutoPlaySpeed(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.transitionAutoPlaySpeed, x => effect.transitionAutoPlaySpeed = x, target, duration).SetTarget(effect);
        }
        #endregion

        #region Target
        public static Tween DOTargetColor(this UIEffect effect, Color target, float duration)
        {
            return DOTween.To(() => effect.targetColor, x => effect.targetColor = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTargetRange(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.targetRange, x => effect.targetRange = x, target, duration).SetTarget(effect);
        }

        public static Tween DOTargetSoftness(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.targetSoftness, x => effect.targetSoftness = x, target, duration).SetTarget(effect);
        }
        #endregion

        #region Shadow
        public static Tween DOShadowDistance(this UIEffect effect, Vector2 target, float duration)
        {
            return DOTween.To(() => effect.shadowDistance, x => effect.shadowDistance = x, target, duration).SetTarget(effect);
        }

        public static Tween DOShadowFade(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.shadowFade, x => effect.shadowFade = x, target, duration).SetTarget(effect);
        }

        public static Tween DOShadowIteration(this UIEffect effect, int target, float duration)
        {
            return DOTween.To(() => effect.shadowIteration, x => effect.shadowIteration = x, target, duration).SetTarget(effect);
        }

        public static Tween DOShadowMirrorScale(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.shadowMirrorScale, x => effect.shadowMirrorScale = x, target, duration).SetTarget(effect);
        }

        public static Tween DOShadowBlurIntensity(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.shadowBlurIntensity, x => effect.shadowBlurIntensity = x, target, duration).SetTarget(effect);
        }

        public static Tween DOShadowColor(this UIEffect effect, Color target, float duration)
        {
            return DOTween.To(() => effect.shadowColor, x => effect.shadowColor = x, target, duration).SetTarget(effect);
        }

        public static Tween DOShadowColorHueShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.shadowColorHueShift, x => effect.shadowColorHueShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOShadowColorSaturationShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.shadowColorSaturationShift, x => effect.shadowColorSaturationShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOShadowColorValueShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.shadowColorValueShift, x => effect.shadowColorValueShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOShadowColorContrastShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.shadowColorContrastShift, x => effect.shadowColorContrastShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOShadowColorBrightnessShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.shadowColorBrightnessShift, x => effect.shadowColorBrightnessShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOShadowColorAlpha(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.shadowColorAlpha, x => effect.shadowColorAlpha = x, target, duration).SetTarget(effect);
        }
        #endregion

        #region Edge
        public static Tween DOEdgeShinyRate(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.edgeShinyRate, x => effect.edgeShinyRate = x, target, duration).SetTarget(effect);
        }

        public static Tween DOEdgeWidth(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.edgeWidth, x => effect.edgeWidth = x, target, duration).SetTarget(effect);
        }

        public static Tween DOEdgeColor(this UIEffect effect, Color target, float duration)
        {
            return DOTween.To(() => effect.edgeColor, x => effect.edgeColor = x, target, duration).SetTarget(effect);
        }

        public static Tween DOEdgeColorHueShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.edgeColorHueShift, x => effect.edgeColorHueShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOEdgeColorSaturationShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.edgeColorSaturationShift, x => effect.edgeColorSaturationShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOEdgeColorValueShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.edgeColorValueShift, x => effect.edgeColorValueShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOEdgeColorContrastShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.edgeColorContrastShift, x => effect.edgeColorContrastShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOEdgeColorBrightnessShift(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.edgeColorBrightnessShift, x => effect.edgeColorBrightnessShift = x, target, duration).SetTarget(effect);
        }

        public static Tween DOEdgeColorAlpha(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.edgeColorAlpha, x => effect.edgeColorAlpha = x, target, duration).SetTarget(effect);
        }

        public static Tween DOEdgeShinyWidth(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.edgeShinyWidth, x => effect.edgeShinyWidth = x, target, duration).SetTarget(effect);
        }

        public static Tween DOEdgeShinyAutoPlaySpeed(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.edgeShinyAutoPlaySpeed, x => effect.edgeShinyAutoPlaySpeed = x, target, duration).SetTarget(effect);
        }
        #endregion

        #region Gradiation
        public static Tween DOGradationIntensity(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.gradationIntensity, x => effect.gradationIntensity = x, target, duration).SetTarget(effect);
        }

        public static Tween DOGradationColor1(this UIEffect effect, Color target, float duration)
        {
            return DOTween.To(() => effect.gradationColor1, x => effect.gradationColor1 = x, target, duration).SetTarget(effect);
        }

        public static Tween DOGradationColor2(this UIEffect effect, Color target, float duration)
        {
            return DOTween.To(() => effect.gradationColor2, x => effect.gradationColor2 = x, target, duration).SetTarget(effect);
        }

        public static Tween DOGradationColor3(this UIEffect effect, Color target, float duration)
        {
            return DOTween.To(() => effect.gradationColor3, x => effect.gradationColor3 = x, target, duration).SetTarget(effect);
        }

        public static Tween DOGradationColor4(this UIEffect effect, Color target, float duration)
        {
            return DOTween.To(() => effect.gradationColor4, x => effect.gradationColor4 = x, target, duration).SetTarget(effect);
        }

        public static Tween DOGradationOffset(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.gradationOffset, x => effect.gradationOffset = x, target, duration).SetTarget(effect);
        }

        public static Tween DOGradationScale(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.gradationScale, x => effect.gradationScale = x, target, duration).SetTarget(effect);
        }

        public static Tween DOGradationRotation(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.gradationRotation, x => effect.gradationRotation = x, target, duration).SetTarget(effect);
        }
        #endregion

        #region Detail
        public static Tween DODetailIntensity(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.detailIntensity, x => effect.detailIntensity = x, target, duration).SetTarget(effect);
        }

        public static Tween DODetailColor(this UIEffect effect, Color target, float duration)
        {
            return DOTween.To(() => effect.detailColor, x => effect.detailColor = x, target, duration).SetTarget(effect);
        }

        public static Tween DODetailColorAlpha(this UIEffect effect, float target, float duration)
        {
            return DOTween.To(() => effect.detailColorAlpha, x => effect.detailColorAlpha = x, target, duration).SetTarget(effect);
        }

        public static Tween DODetailTextureScale(this UIEffect effect, Vector2 target, float duration)
        {
            return DOTween.To(() => effect.detailTextureScale, x => effect.detailTextureScale = x, target, duration).SetTarget(effect);
        }

        public static Tween DODetailTextureSpeed(this UIEffect effect, Vector2 target, float duration)
        {
            return DOTween.To(() => effect.detailTextureSpeed, x => effect.detailTextureSpeed = x, target, duration).SetTarget(effect);
        }
        #endregion
    }
}
