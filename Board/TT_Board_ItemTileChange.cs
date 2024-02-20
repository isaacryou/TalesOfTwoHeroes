using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TMPro;
using TT.Equipment;
using TT.StatusEffect;
using TT.Core;

namespace TT.Board
{
    public enum EquipmentChangeType
    {
        acquire,
        remove,
        removeEnchant
    }

    public class TT_Board_ItemTileChange: MonoBehaviour
    {
        public Canvas itemTileChangeCanvas;
        public Vector3 itemTileStartLocation;
        public float itemTileDistanceX;
        private GameObject parentObject;
        private GameObject weaponButton;
        private GameObject equipmentObject;
        private int equipmentChangeType;

        public Image cardImage;
        public Image cardWeaponSlotImage;
        public Image cardWeaponImage;
        public TMP_Text titleText;
        public TMP_Text descriptionText;
        public Image enchantIconImage;
        public Image enchantIconFrameImage;

        public Renderer sparkEffectRenderer;

        private readonly float ITEM_TILE_MOVE_TIME = 0.7f;

        private readonly float ITEM_TILE_START_SCALE = 0.35f;
        private readonly float ITEM_TILE_MIDDLE_SCALE = 0.2f;
        private readonly float ITEM_TILE_END_SCALE = 0.001f;

        private readonly float ITEM_TILE_MIDDLE_SCALE_TIME = 0.1f;

        private readonly float ITEM_TILE_FADE_IN_TIME = 0.6f;
        private readonly float ITEM_TILE_FADE_IN_DISTANCE_Y = 60f;
        private readonly float ITEM_TILE_WAIT_BEFORE_MOVING = 0.5f;
        private readonly float ITEM_TILE_ENCHANT_REMOVE_TIME = 0.3f;
        private readonly float ITEM_TILE_ENCHANT_WAIT_AFTER_REMOVE_TIME = 1.2f;

        public GameObject arsenalDestroyEffect;
        public Vector3 arsenalDestroyEffectLocation;

        public GameObject arsenalRemoveEnchantEffect;
        public Vector3 arsenalRemoveEnchantEffectLocation;

        public Canvas weaponSlotCanvas;

        public Sprite cardTierOneSprite;
        public Sprite cardTierTwoSprite;
        public Sprite cardTierThreeSprite;

        public TrailRenderer trailRenderer;
        private readonly float TRAIL_START_WIDTH = 1.2f;

        public GameObject cardBlackoutPrefab;
        private readonly float CARD_BLACKOUT_TIME = 0.7f;
        private readonly float CARD_WAIT_BEFORE_FADE_TIME = 0.1f;
        private readonly float CARD_FADE_TIME = 1f;
        public AudioClip audioClipToPlayOnArsenalDestroy;
        public AudioClip audioClipToPlayOnArsenalFade;
        public AudioSource audioSourceToUse;
        private readonly float PLAY_ARSENAL_DESTROY_AFTER_TIME = 0.2f;
        private readonly float PLAY_ARSENAL_FADE_AFTER_TIME = 0.13f;

        private int itemCount;

        public void InitializeItemTileChange(TT_Player_Player _playerScript, int _itemCount, GameObject _equipmentObject, int _equipmentChangeType)
        {
            parentObject = _playerScript.equipmentChangeParent;
            weaponButton = _playerScript.weaponButton;
            equipmentObject = _equipmentObject;
            equipmentChangeType = _equipmentChangeType;

            itemCount = _itemCount;

            transform.localPosition = new Vector3(itemTileStartLocation.x + ((_itemCount-1) * itemTileDistanceX), itemTileStartLocation.y, itemTileStartLocation.z);

            UpdateSortingOrder(0);

            UpdateTileByEquipment();

            StartCoroutine(ChangeTileAnimation());
        }

        private void UpdateTileByEquipment()
        {
            TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();
            Sprite equipmentSprite = equipmentScript.equipmentSprite;
            string equipmentTitle = equipmentScript.equipmentName;
            string equipmentDescription = equipmentScript.GetEquipmentDescription();

            float equipmentSpriteX = equipmentScript.equipmentSpriteX;
            float equipmentSpriteY = equipmentScript.equipmentSpriteY;
            float equipmentWidth = equipmentScript.equipmentSpriteWidth;
            float equipmentHeight = equipmentScript.equipmentSpriteHeight;
            float equipmentScaleX = equipmentScript.equipmentScaleX;
            float equipmentScaleY = equipmentScript.equipmentScaleY;
            Vector3 equipmentRotation = equipmentScript.equipmentRotation;

            int equipmentLevel = equipmentScript.equipmentLevel;
            if (equipmentLevel == 1)
            {
                cardImage.sprite = cardTierOneSprite;
            }
            else if (equipmentLevel == 2)
            {
                cardImage.sprite = cardTierTwoSprite;
            }
            else
            {
                cardImage.sprite = cardTierThreeSprite;
            }

            RectTransform mainSpriteObjectRect = cardWeaponImage.gameObject.GetComponent<RectTransform>();
            mainSpriteObjectRect.transform.localPosition = new Vector3(equipmentSpriteX, equipmentSpriteY, 0);
            mainSpriteObjectRect.sizeDelta = new Vector2(equipmentWidth, equipmentHeight);
            mainSpriteObjectRect.localScale = new Vector2(equipmentScaleX, equipmentScaleY);
            mainSpriteObjectRect.rotation = Quaternion.Euler(equipmentRotation);

            cardWeaponImage.sprite = equipmentSprite;
            titleText.text = equipmentTitle;
            descriptionText.text = equipmentDescription;

            if (equipmentScript.enchantObject != null)
            {
                TT_StatusEffect_ATemplate enchantTemplate = equipmentScript.enchantObject.GetComponent<TT_StatusEffect_ATemplate>();
                Sprite enchantIcon = enchantTemplate.GetStatusEffectIcon();
                enchantIconImage.sprite = enchantIcon;
                Vector2 itemTileEnchantImageSize = enchantTemplate.GetStatusEffectIconSize();
                RectTransform enchantIconRectTransform = enchantIconImage.gameObject.GetComponent<RectTransform>();
                enchantIconRectTransform.sizeDelta = itemTileEnchantImageSize;
                enchantIconFrameImage.gameObject.SetActive(true);
            }
        }

        IEnumerator ChangeTileAnimation()
        {
            Vector3 startLocation = new Vector3(transform.localPosition.x, transform.localPosition.y - ITEM_TILE_FADE_IN_DISTANCE_Y, transform.localPosition.z);
            Vector3 targetLocation = transform.localPosition;

            float timeElapsed = 0;
            while (timeElapsed < ITEM_TILE_FADE_IN_TIME)
            {
                float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, ITEM_TILE_FADE_IN_TIME);
                transform.localPosition = Vector3.Lerp(startLocation, targetLocation, smoothCurbTime);

                float smoothCurbFadeTime = timeElapsed / ITEM_TILE_FADE_IN_TIME;
                float createdTileImageAlpha = Mathf.Lerp(0f, 1f, smoothCurbFadeTime);
                cardImage.color = new Color(1f, 1f, 1f, createdTileImageAlpha);
                cardWeaponSlotImage.color = new Color(1f, 1f, 1f, createdTileImageAlpha);
                cardWeaponImage.color = new Color(1f, 1f, 1f, createdTileImageAlpha);
                titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, createdTileImageAlpha);
                descriptionText.color = new Color(descriptionText.color.r, descriptionText.color.g, descriptionText.color.b, createdTileImageAlpha);
                enchantIconImage.color = new Color(1f, 1f, 1f, createdTileImageAlpha);
                enchantIconFrameImage.color = new Color(1f, 1f, 1f, createdTileImageAlpha);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            cardImage.color = new Color(1f, 1f, 1f, 1f);
            cardWeaponSlotImage.color = new Color(1f, 1f, 1f, 1f);
            cardWeaponImage.color = new Color(1f, 1f, 1f, 1f);
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 1f);
            descriptionText.color = new Color(descriptionText.color.r, descriptionText.color.g, descriptionText.color.b, 1f);
            enchantIconImage.color = new Color(1f, 1f, 1f, 1f);
            enchantIconFrameImage.color = new Color(1f, 1f, 1f, 1f);

            yield return new WaitForSeconds(ITEM_TILE_WAIT_BEFORE_MOVING);

            Transform weaponButtonParentTransform = weaponButton.transform.parent;

            if (equipmentChangeType == (int)EquipmentChangeType.acquire)
            {
                Vector3 weaponButtonParentLocation = weaponButtonParentTransform.localPosition;
                Vector3 weaponButtonLocation = weaponButton.transform.localPosition + weaponButtonParentLocation;
                Vector3 currentWorldLocation = transform.localPosition;

                float yDifference = weaponButtonLocation.y - currentWorldLocation.y;
                float xDifference = weaponButtonLocation.x - currentWorldLocation.x;
                float angleBetweenTwoPoints = 90+(Mathf.Atan2(yDifference, xDifference) * Mathf.Rad2Deg);

                List<Vector2> controlPoints = new List<Vector2>();
                Vector2 firstControlPoint = transform.localPosition;
                controlPoints.Add(firstControlPoint);
                Vector2 secondControlPoint = new Vector2(firstControlPoint.x - 400, firstControlPoint.y + 600);
                controlPoints.Add(secondControlPoint);
                Vector2 thirdControlPoint = new Vector2(firstControlPoint.x - 1000, firstControlPoint.y - 600);
                controlPoints.Add(thirdControlPoint);
                Vector2 fourthControlPoint = new Vector2(firstControlPoint.x + 400, firstControlPoint.y -200);
                controlPoints.Add(fourthControlPoint);
                Vector2 fifthhControlPoint = (Vector2)weaponButtonLocation;
                controlPoints.Add(fifthhControlPoint);

                /*
                List<Vector2> controlPoints = new List<Vector2>();
                Vector2 firstControlPoint = transform.position;
                controlPoints.Add(transform.position);
                Vector2 secondControlPoint = new Vector2(firstControlPoint.x - 200, firstControlPoint.y + 400);
                controlPoints.Add(secondControlPoint);
                Vector2 thirdControlPoint = new Vector2(firstControlPoint.x - 800, firstControlPoint.y - 500);
                controlPoints.Add(thirdControlPoint);
                Vector2 fourthControlPoint = new Vector2(firstControlPoint.x + 400, firstControlPoint.y - 200);
                controlPoints.Add(fourthControlPoint);
                Vector2 fifthhControlPoint = (Vector2)weaponButtonLocation;
                controlPoints.Add(fifthhControlPoint);
                */

                float interval = 0.01f;
                List<Vector2> curve = BezierCurve.PointList2(controlPoints, interval);

                float indexMultiplier = 1 / ITEM_TILE_MOVE_TIME;

                EnableTrail();

                timeElapsed = 0;
                while(timeElapsed < ITEM_TILE_MOVE_TIME)
                {
                    if (timeElapsed < ITEM_TILE_MIDDLE_SCALE_TIME)
                    {
                        float middleScaleCurb = timeElapsed / ITEM_TILE_MIDDLE_SCALE_TIME;
                        float middleScale = Mathf.Lerp(ITEM_TILE_START_SCALE, ITEM_TILE_MIDDLE_SCALE, middleScaleCurb);

                        transform.localScale = new Vector3(middleScale, middleScale, 1f);
                    }
                    else
                    {
                        float endScaleCurb = (timeElapsed - ITEM_TILE_MIDDLE_SCALE_TIME) / (ITEM_TILE_MOVE_TIME - ITEM_TILE_MIDDLE_SCALE_TIME);
                        float endScale = Mathf.Lerp(ITEM_TILE_MIDDLE_SCALE, ITEM_TILE_END_SCALE, endScaleCurb);

                        transform.localScale = new Vector3(endScale, endScale, 1f);
                    }

                    int index = (int)((timeElapsed / interval) * indexMultiplier);

                    if (index < 0)
                    {
                        index = 0;
                    }
                    else if (index >= curve.Count-1)
                    {
                        index = curve.Count - 1;
                    }

                    Vector2 newLocation = curve[index];
                    Vector2 currentLocation = (Vector2)transform.localPosition;

                    transform.localPosition = new Vector3(newLocation.x, newLocation.y, transform.localPosition.z);

                    if (newLocation != currentLocation)
                    {
                        Vector2 currentAngleLocation = new Vector2(0f, System.Math.Abs(currentLocation.y));
                        Vector2 targetAngleLocation = new Vector2(newLocation.x - currentLocation.x, newLocation.y - System.Math.Abs(currentLocation.y));
                        float angleToRotate = Vector2.SignedAngle(currentAngleLocation, targetAngleLocation);
                        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angleToRotate);

                        /*
                        if (itemCount == 2)
                        {
                            Instantiate(gameObject, transform.parent);

                            Debug.Log("Item count: " + itemCount + " ; Current location: " + currentLocation.ToString() + " ; Target location: " + newLocation.ToString() + " ; Angle To Rotate: " + angleToRotate);
                        }
                        */
                    }

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                transform.localScale = new Vector3(0f, 0f, 1f);

                yield return new WaitForSeconds(0.5f);
            }
            else if (equipmentChangeType == (int)EquipmentChangeType.removeEnchant)
            {
                GameObject createdArsenalRemoveEnchantEffect = Instantiate(arsenalRemoveEnchantEffect, transform);
                createdArsenalRemoveEnchantEffect.transform.localPosition = arsenalRemoveEnchantEffectLocation;

                yield return new WaitForSeconds(ITEM_TILE_ENCHANT_REMOVE_TIME);

                enchantIconImage.color = new Color(1f, 1f, 1f, 0f);
                enchantIconFrameImage.color = new Color(1f, 1f, 1f, 0f);

                yield return new WaitForSeconds(ITEM_TILE_ENCHANT_WAIT_AFTER_REMOVE_TIME);

                Vector3 weaponButtonParentLocation = weaponButtonParentTransform.localPosition;
                Vector3 weaponButtonLocation = weaponButton.transform.localPosition + weaponButtonParentLocation;
                Vector3 currentWorldLocation = transform.localPosition;

                float yDifference = weaponButtonLocation.y - currentWorldLocation.y;
                float xDifference = weaponButtonLocation.x - currentWorldLocation.x;
                float angleBetweenTwoPoints = 90 + (Mathf.Atan2(yDifference, xDifference) * Mathf.Rad2Deg);

                List<Vector2> controlPoints = new List<Vector2>();
                Vector2 firstControlPoint = transform.localPosition;
                controlPoints.Add(firstControlPoint);
                Vector2 secondControlPoint = new Vector2(firstControlPoint.x - 400, firstControlPoint.y + 600);
                controlPoints.Add(secondControlPoint);
                Vector2 thirdControlPoint = new Vector2(firstControlPoint.x - 1000, firstControlPoint.y - 600);
                controlPoints.Add(thirdControlPoint);
                Vector2 fourthControlPoint = new Vector2(firstControlPoint.x + 400, firstControlPoint.y - 200);
                controlPoints.Add(fourthControlPoint);
                Vector2 fifthhControlPoint = (Vector2)weaponButtonLocation;
                controlPoints.Add(fifthhControlPoint);

                /*
                List<Vector2> controlPoints = new List<Vector2>();
                Vector2 firstControlPoint = transform.position;
                controlPoints.Add(transform.position);
                Vector2 secondControlPoint = new Vector2(firstControlPoint.x - 200, firstControlPoint.y + 400);
                controlPoints.Add(secondControlPoint);
                Vector2 thirdControlPoint = new Vector2(firstControlPoint.x - 800, firstControlPoint.y - 500);
                controlPoints.Add(thirdControlPoint);
                Vector2 fourthControlPoint = new Vector2(firstControlPoint.x + 400, firstControlPoint.y - 200);
                controlPoints.Add(fourthControlPoint);
                Vector2 fifthhControlPoint = (Vector2)weaponButtonLocation;
                controlPoints.Add(fifthhControlPoint);
                */

                float interval = 0.01f;
                List<Vector2> curve = BezierCurve.PointList2(controlPoints, interval);

                float indexMultiplier = 1 / ITEM_TILE_MOVE_TIME;

                EnableTrail();

                timeElapsed = 0;
                while (timeElapsed < ITEM_TILE_MOVE_TIME)
                {
                    if (timeElapsed < ITEM_TILE_MIDDLE_SCALE_TIME)
                    {
                        float middleScaleCurb = timeElapsed / ITEM_TILE_MIDDLE_SCALE_TIME;
                        float middleScale = Mathf.Lerp(ITEM_TILE_START_SCALE, ITEM_TILE_MIDDLE_SCALE, middleScaleCurb);

                        transform.localScale = new Vector3(middleScale, middleScale, 1f);
                    }
                    else
                    {
                        float endScaleCurb = (timeElapsed - ITEM_TILE_MIDDLE_SCALE_TIME) / (ITEM_TILE_MOVE_TIME - ITEM_TILE_MIDDLE_SCALE_TIME);
                        float endScale = Mathf.Lerp(ITEM_TILE_MIDDLE_SCALE, ITEM_TILE_END_SCALE, endScaleCurb);

                        transform.localScale = new Vector3(endScale, endScale, 1f);
                    }

                    int index = (int)((timeElapsed / interval) * indexMultiplier);

                    if (index < 0)
                    {
                        index = 0;
                    }
                    else if (index >= curve.Count - 1)
                    {
                        index = curve.Count - 1;
                    }

                    Vector2 newLocation = curve[index];
                    Vector2 currentLocation = (Vector2)transform.localPosition;

                    transform.localPosition = new Vector3(newLocation.x, newLocation.y, transform.localPosition.z);

                    if (newLocation != currentLocation)
                    {
                        Vector2 currentAngleLocation = new Vector2(0f, System.Math.Abs(currentLocation.y));
                        Vector2 targetAngleLocation = new Vector2(newLocation.x - currentLocation.x, newLocation.y - System.Math.Abs(currentLocation.y));
                        float angleToRotate = Vector2.SignedAngle(currentAngleLocation, targetAngleLocation);
                        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angleToRotate);

                        /*
                        if (itemCount == 2)
                        {
                            Instantiate(gameObject, transform.parent);

                            Debug.Log("Item count: " + itemCount + " ; Current location: " + currentLocation.ToString() + " ; Target location: " + newLocation.ToString() + " ; Angle To Rotate: " + angleToRotate);
                        }
                        */
                    }

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                transform.localScale = new Vector3(0f, 0f, 1f);

                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                GameObject createdCardBlackout = Instantiate(cardBlackoutPrefab, transform);
                TT_Board_BoardItemTileBlackout cardBlackoutScript = createdCardBlackout.GetComponent<TT_Board_BoardItemTileBlackout>();

                bool arsenalDestroySoundPlayed = false;

                timeElapsed = 0;
                while(timeElapsed < CARD_BLACKOUT_TIME)
                {
                    if (timeElapsed > PLAY_ARSENAL_DESTROY_AFTER_TIME && !arsenalDestroySoundPlayed)
                    {
                        arsenalDestroySoundPlayed = true;

                        audioSourceToUse.clip = audioClipToPlayOnArsenalDestroy;
                        audioSourceToUse.Play();
                    }

                    float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, CARD_BLACKOUT_TIME);

                    cardBlackoutScript.MoveShadows(smoothCurb);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                cardBlackoutScript.MoveShadows(1f);

                cardImage.color = new Color(1f, 1f, 1f, 0f);
                cardWeaponSlotImage.color = new Color(1f, 1f, 1f, 0f);
                cardWeaponImage.color = new Color(1f, 1f, 1f, 0f);
                titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0f);
                descriptionText.color = new Color(descriptionText.color.r, descriptionText.color.g, descriptionText.color.b, 0f);
                enchantIconImage.color = new Color(1f, 1f, 1f, 0f);
                enchantIconFrameImage.color = new Color(1f, 1f, 1f, 0f);
                
                yield return new WaitForSeconds(CARD_WAIT_BEFORE_FADE_TIME);

                timeElapsed = 0;
                while(timeElapsed < CARD_FADE_TIME)
                {
                    float fixedCurb = timeElapsed / CARD_FADE_TIME;

                    cardBlackoutScript.SetRendererFade(fixedCurb);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }
            }

            DestroyTile();
        }

        private void DestroyTile()
        {
            Destroy(gameObject);
        }

        private void UpdateSortingOrder(int _sortingOrder)
        {
            itemTileChangeCanvas.overrideSorting = true;
            itemTileChangeCanvas.sortingLayerName = "EquipmentChange";
            itemTileChangeCanvas.sortingOrder = _sortingOrder;

            weaponSlotCanvas.overrideSorting = true;
            weaponSlotCanvas.sortingLayerName = "EquipmentChange";
            weaponSlotCanvas.sortingOrder = _sortingOrder - 1;

            trailRenderer.sortingLayerName = "EquipmentChange";
            trailRenderer.sortingOrder = _sortingOrder + 10;
        }

        private void EnableTrail()
        {
            trailRenderer.gameObject.SetActive(true);
        }
    }
}
