using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpeedAndTimeWaitingLibrary
{
    // *** Speeds *** //

    public float fruitStartSpeed=10f;
    public float fruitMaxSpeed=25f;
    public float fruitSwipeSpeed=24f;

    public float harvesterSpeed=20f;
    public float boomerangSpeed=10f;

    public float fruitAccelerationMultiplier = 15f;

    public float powerUpCreationFruitSpeed = 5f;

    // *** Time durations *** //

    public float powerUpCreationFruitGatheringDuration = 0.19f;
    public float beforeTwoPowerUpMergeWait = 0.1f;
    public float fruitSwipeDuration = 0.18f;
    public float afterReverseWait = 0.1f;

    public float fruitDestroyAnimDuration = 0.1f;
    public float waitBeforeClosingFruitSprite = 0.026f;
    public float waitBeforeRemovingFruitFromBoard = 0.020f;

    public float maxFruitParticleDuration = 2f;
    public float waitBeforeFallingOtherFruit = 0.04f;
    public float waitBeforeCreatingFruitsTopOfBoard = 0.1f;

    public float createdPowerupUninteractableDuration=0.3f;

    public float twoTNTMergeAnimDuration = 1.44f;
    public float twoTNTMergeColumnStopDuration = 1.76f;

    public float twoBoomerangMergeAnimDuration = 1.34f;
    public float twoBoomerangMergeColumnStopDuration = 1.34f;

    public float twoDiscoballMergeAnimDuration = 3.8f;
    public float twoDiscoBallMergeColumnStopDuration = 4.8f;
    public float twoDiscoBallMergeExplosionDuration = 1.5f;

    public float harvesterTNTMergeAnimDuration = 0.56f;

    // this variable multipys with witdh or height so for every tile on column or row it stopst that column(s).
    // This variable used by vertical and horizontal not seperated.
    public float harvesterColumnStopDurationMultiplier = 0.06f;

    public float singleTNTColumnStopDuration = 0.29f;

    public float shuffleAfterDestroyBeforeCreationWait = 0.5f;

    public float discoballWaitBetweenMarkingFruits = 0.1f;

    public float discoballWaitBetweenDestroyingMarkedFruits = 0.01f;
    public float discoballWaitBetweenActivatingMarkedPowerUps = 0.01f;
    public float waitForEffectOfSingleUsedDiscoball = 1.5f;

    public float sunflowerCreationAnimDuration = 0.5f;
    public float sunflowerColumnStopDurationMultiplier = 0.06f;
    public float waitBeforeDestroyingSunflowerGrassAfterActivated = 0.1f;
    public float waitBeforeDestroyingSunflower = 1f;

    public float waitBeforeDestroyingTNT = 0.1f;
}
