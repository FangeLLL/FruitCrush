using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectSpeedAndTimeWaitingNameSpace {
    public class ObjectSpeedAndTimeWaitingLibrary
    {
        // *** Speeds *** //

        public static float fruitStartSpeed = 10f;
        public static float fruitMaxSpeed = 25f;
        public static float fruitSwipeSpeed = 24f;

        public static float harvesterSpeed = 20f;
        public static float boomerangSpeed = 10f;

        public static float fruitAccelerationMultiplier = 15f;

        public static float powerUpCreationFruitSpeed = 5f;

        public static float lighteningSpeed = 20f;

        // *** Time durations *** //

        public static float powerUpCreationFruitGatheringDuration = 0.17f;
        public static float beforeTwoPowerUpMergeWait = 0.1f;
        public static float fruitSwipeDuration = 0.18f;
        public static float afterReverseWait = 0.1f;

        public static float fruitDestroyAnimDuration = 0.1f;
        public static float waitBeforeClosingFruitSprite = 0.026f;
        public static float waitBeforeRemovingFruitFromBoard = 0.020f;

        public static float maxFruitParticleDuration = 2f;
        public static float waitBeforeFallingOtherFruit = 0.04f;
        public static float waitBeforeCreatingFruitsTopOfBoard = 0.1f;

        //  public float createdPowerupUninteractableDuration=0.01f;

        public static float twoTNTMergeAnimDuration = 1.63f;
        public static float twoTNTMergeColumnStopDuration = 1.76f;

        public static float twoBoomerangMergeAnimDuration = 1.34f;
        public static float twoBoomerangMergeColumnStopDuration = 1.34f;

        public static float twoDiscoballMergeAnimDuration = 3.2f;
        public static float twoDiscoBallMergeColumnStopDuration = 4.1f;
        public static float twoDiscoBallMergeExplosionDuration = 1.2f;
        public static float twoDiscoBallMergeBoardFillAfterExplosionStart = 0.5f;

        public static float afterLastLighteningWaitTimeBeforeDestroyingFruits = 0.1f;

        public static float harvesterTNTMergeAnimDuration = 0.35f;

        // this variable multipys with witdh or height so for every tile on column or row it stopst that column(s).
        // This variable used by vertical and horizontal not seperated.
        public static float verticalHarvesterColumnStopDurationMultiplier = 0.065f;
        public static float horizontalHarvesterColumnStopDurationMultiplier = 0.065f;

        public static float singleTNTColumnStopDuration = 0.29f;

        public static float shuffleAfterDestroyBeforeCreationWait = 0.5f;

        public static float discoballWaitBetweenMarkingFruits = 0.1f;

        public static float discoballWaitBetweenDestroyingMarkedFruits = 0.01f;
        public static float discoballWaitBetweenActivatingMarkedPowerUps = 0.01f;
        public static float waitForEffectOfSingleUsedDiscoball = 1.5f;

        public static float sunflowerCreationAnimDuration = 0.5f;
        public static float sunflowerColumnStopDurationMultiplier = 0.06f;
        public static float waitBeforeDestroyingSunflowerGrassAfterActivated = 0.1f;
        public static float waitBeforeDestroyingSunflower = 1f;

        public static float waitBeforeDestroyingTNT = 0.024f;
        public static float waitOneFrame = 0.024f;

        // *** Light Configurations *** //

        public static float dimLightIntensity = 0.3f;

    }
}

