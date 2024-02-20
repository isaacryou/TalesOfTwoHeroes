using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TMPro;
using TT.Equipment;
using UnityEngine.UI;
using TT.Core;
using TT.Shop;

namespace TT.Battle
{
    [System.Serializable]
    public class BattleButtonSpecial
    {
        public GameObject specialEffect;
        public Vector3 effectLocation;
        public Vector2 effectScale;
    }

    public class TT_Battle_ButtonController: MonoBehaviour
    {
        public GameObject attackButton;
        public UiScaleOnHover attackButtonScaleOnHover;
        public Button attackButtonComponent;
        public GameObject defenseButton;
        public UiScaleOnHover defenseButtonScaleOnHover;
        public Button defenseButtonComponent;
        public GameObject utilityButton;
        public UiScaleOnHover utilityButtonScaleOnHover;
        public Button utilityButtonComponent;

        private EquipmentXMLSerializer equipmentSerializer;

        private Vector3 attackTileOriginalLocation;
        private Vector3 defenseTileOriginalLocation;
        private Vector3 utilityTileOriginalLocation;

        public Vector3 displayLocationOffset;

        public Material greyScaleMaterial;

        public Color disabledTextColor;
        public Color enabledTextColor;

        public GameObject acceptButton;
        public Button acceptButtonComponent;
        public UiScaleOnHover acceptButtonScaleOnHover;
        public TMP_Text acceptButtonText;

        public BattleButtonSpecial evilSpecialEffect;

        private TT_Battle_ActionTile currentActionTile;

        private readonly int BATTLE_ACTION_TILE_CONFIRM_TEXT_ID = 71;

        public TMP_Text attackTileName;
        public TMP_Text attackTileDescription;
        public TT_Battle_ButtonDescriptionAutoScroll attackTileAutoScroll;
        public TMP_Text defenseTileName;
        public TMP_Text defenseTileDescription;
        public TT_Battle_ButtonDescriptionAutoScroll defenseTileAutoScroll;
        public TMP_Text utilityTileName;
        public TMP_Text utilityTileDescription;
        public TT_Battle_ButtonDescriptionAutoScroll utilityTileAutoScroll;

        void Start()
        {
            attackTileOriginalLocation = attackButton.transform.localPosition;
            defenseTileOriginalLocation = defenseButton.transform.localPosition;
            utilityTileOriginalLocation = utilityButton.transform.localPosition;

            string battleActionTileConfirmString = StringHelper.GetStringFromTextFile(BATTLE_ACTION_TILE_CONFIRM_TEXT_ID);

            if (acceptButtonText != null)
            {
                acceptButtonText.text = battleActionTileConfirmString;
            }
        }

        public GameObject GetButtonByActionId(int _actionTypeId)
        {
            if (_actionTypeId == 0)
            {
                return attackButton;
            }
            else if (_actionTypeId == 1)
            {
                return defenseButton;
            }
            else if (_actionTypeId == 2)
            {
                return utilityButton;
            }

            return null;
        }

        public List<GameObject> GetButtonsOtherThanActionId(int _actionTypeId)
        {
            List<GameObject> result = new List<GameObject>();

            if (_actionTypeId != 0)
            {
                result.Add(attackButton);
            }
            
            if (_actionTypeId != 1)
            {
                result.Add(defenseButton);
            }
           
            if (_actionTypeId != 2)
            {
                result.Add(utilityButton);
            }

            return result;
        }

        public void ChangeActionButtonWithoutFlip(TT_Equipment_Equipment _equipmentScript, TT_Battle_ActionTile _actionTile = null, TT_Board_ItemTile _boardItemTile = null, TT_Shop_ItemTile _shopItemTile = null)
        {
            foreach(Transform child in attackButton.transform)
            {
                if (child.gameObject.tag == "SpecialEffect")
                {
                    child.gameObject.SetActive(false);
                    Destroy(child.gameObject);
                }
            }

            foreach (Transform child in defenseButton.transform)
            {
                if (child.gameObject.tag == "SpecialEffect")
                {
                    child.gameObject.SetActive(false);
                    Destroy(child.gameObject);
                }
            }

            foreach (Transform child in utilityButton.transform)
            {
                if (child.gameObject.tag == "SpecialEffect")
                {
                    child.gameObject.SetActive(false);
                    Destroy(child.gameObject);
                }
            }

            EnableActionButton(attackButton);
            EnableActionButton(defenseButton);
            EnableActionButton(utilityButton);

            UpdateActionButtonTooltips(_equipmentScript);

            if (_actionTile != null)
            {
                currentActionTile = _actionTile;

                if (_actionTile.attackSpecialId != null && _actionTile.attackSpecialId.Count > 0)
                {
                    UpdateActionTileSpecial(_actionTile.attackSpecialId, attackButton);
                }
                
                if (_actionTile.defenseSpecialId != null && _actionTile.defenseSpecialId.Count > 0)
                {
                    UpdateActionTileSpecial(_actionTile.defenseSpecialId, defenseButton);
                }

                if (_actionTile.utilitySpecialId != null && _actionTile.utilitySpecialId.Count > 0)
                {
                    UpdateActionTileSpecial(_actionTile.utilitySpecialId, utilityButton);
                }
            }
            else if (_boardItemTile != null)
            {
                if (_boardItemTile.attackSpecialId != null && _boardItemTile.attackSpecialId.Count > 0)
                {
                    UpdateActionTileSpecial(_boardItemTile.attackSpecialId, attackButton);
                }

                if (_boardItemTile.defenseSpecialId != null && _boardItemTile.defenseSpecialId.Count > 0)
                {
                    UpdateActionTileSpecial(_boardItemTile.defenseSpecialId, defenseButton);
                }

                if (_boardItemTile.utilitySpecialId != null && _boardItemTile.utilitySpecialId.Count > 0)
                {
                    UpdateActionTileSpecial(_boardItemTile.utilitySpecialId, utilityButton);
                }
            }
            else if (_shopItemTile != null)
            {
                if (_shopItemTile.attackSpecialId != null && _shopItemTile.attackSpecialId.Count > 0)
                {
                    UpdateActionTileSpecial(_shopItemTile.attackSpecialId, attackButton);
                }

                if (_shopItemTile.defenseSpecialId != null && _shopItemTile.defenseSpecialId.Count > 0)
                {
                    UpdateActionTileSpecial(_shopItemTile.defenseSpecialId, defenseButton);
                }

                if (_shopItemTile.utilitySpecialId != null && _shopItemTile.utilitySpecialId.Count > 0)
                {
                    UpdateActionTileSpecial(_shopItemTile.utilitySpecialId, utilityButton);
                }
            }
        }

        //This is called to update the tooltip of the button
        //If the action buttons needs to be flipped, call this after they have been flipped
        private void UpdateActionButtonTooltips(TT_Equipment_Equipment _equipmentScript)
        {
            int equipmentId = _equipmentScript.equipmentId;

            if (equipmentSerializer == null)
            {
                equipmentSerializer = new EquipmentXMLSerializer();
                equipmentSerializer.InitializeEquipmentFile();
            }

            string attackTitleToUpdate = equipmentSerializer.GetStringValueFromEquipment(equipmentId, "offenseName");
            string attackDescriptionToUpdate = _equipmentScript.GetOffenseActionDescription();
            string defenseTitleToUpdate = equipmentSerializer.GetStringValueFromEquipment(equipmentId, "defenseName");
            string defenseDescriptionToUpdate = _equipmentScript.GetDefenseActionDescription();
            string utilityTitleToUpdate = equipmentSerializer.GetStringValueFromEquipment(equipmentId, "utilityName");
            string utilityDescriptionToUpdate = _equipmentScript.GetUtilityActionDescription();

            attackTileName.text = attackTitleToUpdate;
            attackTileDescription.text = attackDescriptionToUpdate;
            attackTileAutoScroll.TextGotUpdated();
            defenseTileName.text = defenseTitleToUpdate;
            defenseTileDescription.text = defenseDescriptionToUpdate;
            defenseTileAutoScroll.TextGotUpdated();
            utilityTileName.text = utilityTitleToUpdate;
            utilityTileDescription.text = utilityDescriptionToUpdate;
            utilityTileAutoScroll.TextGotUpdated();

            //If this got updated, it means a new arsenal has been clicked
            if (acceptButtonText != null)
            {
                acceptButtonText.color = disabledTextColor;
            }
        }

        private void UpdateActionTileSpecial(List<int> _statusEffectId, GameObject _actionTile)
        {
            if (_statusEffectId.Contains(66))
            {
                GameObject evilEffect = Instantiate(evilSpecialEffect.specialEffect, _actionTile.transform);
                evilEffect.transform.localPosition = evilSpecialEffect.effectLocation;
                evilEffect.transform.localScale = evilSpecialEffect.effectScale;
                evilEffect.tag = "SpecialEffect";
            }

            if (_statusEffectId.Contains(89))
            {
                DisableActionButton(_actionTile);
            }
        }

        public void MoveActionTileToOriginalLocation()
        {
            attackButton.transform.localPosition = attackTileOriginalLocation;
            defenseButton.transform.localPosition = defenseTileOriginalLocation;
            utilityButton.transform.localPosition = utilityTileOriginalLocation;

            attackButton.gameObject.SetActive(true);
            defenseButton.gameObject.SetActive(true);
            utilityButton.gameObject.SetActive(true);
        }

        //0 = attack, 1 = defense, utility = 2
        public void MoveActionTileToDisplayLocation(int _tileId, Vector3 _baseLocation)
        {
            Vector3 targetLocation = _baseLocation + displayLocationOffset;

            if (_tileId == 0)
            {
                attackButton.transform.localPosition = targetLocation;
                attackButton.gameObject.SetActive(true);
                defenseButton.gameObject.SetActive(false);
                utilityButton.gameObject.SetActive(false);

                attackTileAutoScroll.TextGotUpdated();
                defenseTileAutoScroll.TurnOffCoroutine();
                utilityTileAutoScroll.TurnOffCoroutine();
            }
            else if (_tileId == 1)
            {
                defenseButton.transform.localPosition = targetLocation;
                defenseButton.gameObject.SetActive(true);
                attackButton.gameObject.SetActive(false);
                utilityButton.gameObject.SetActive(false);

                attackTileAutoScroll.TurnOffCoroutine();
                defenseTileAutoScroll.TextGotUpdated();
                utilityTileAutoScroll.TurnOffCoroutine();
            }
            else
            {
                utilityButton.transform.localPosition = targetLocation;
                utilityButton.gameObject.SetActive(true);
                attackButton.gameObject.SetActive(false);
                defenseButton.gameObject.SetActive(false);

                attackTileAutoScroll.TurnOffCoroutine();
                defenseTileAutoScroll.TurnOffCoroutine();
                utilityTileAutoScroll.TextGotUpdated();
            }
        }

        private void EnableActionButton(GameObject _actionButton)
        {
            Image actionButtonImage = _actionButton.GetComponent<Image>();
            Button actionButtonButton = _actionButton.GetComponent<Button>();
            UiScaleOnHover actionButtonUiScaleOnHover = _actionButton.GetComponent<UiScaleOnHover>();

            actionButtonImage.material = null;
            actionButtonButton.interactable = true;
            if (actionButtonUiScaleOnHover != null)
            {
                actionButtonUiScaleOnHover.shouldScaleOnHover = true;
            }
        }

        private void DisableActionButton(GameObject _actionButton)
        {
            Image actionButtonImage = _actionButton.GetComponent<Image>();
            Button actionButtonButton = _actionButton.GetComponent<Button>();
            UiScaleOnHover actionButtonUiScaleOnHover = _actionButton.GetComponent<UiScaleOnHover>();

            TMP_Text actionButtonText = _actionButton.transform.GetChild(0).GetComponent<TMP_Text>();

            actionButtonImage.material = greyScaleMaterial;
            actionButtonButton.interactable = false;
            if (actionButtonUiScaleOnHover != null)
            {
                actionButtonUiScaleOnHover.shouldScaleOnHover = false;
            }
        }

        public void TileSelectedInBattle(bool _isCurrentPlayerTile)
        {
            if (_isCurrentPlayerTile)
            {
                acceptButton.SetActive(true);
                acceptButtonComponent.interactable = false;
                acceptButtonScaleOnHover.shouldScaleOnHover = false;
                acceptButtonText.color = disabledTextColor;
            }
            else
            {
                attackButtonScaleOnHover.shouldScaleOnHover = false;
                defenseButtonScaleOnHover.shouldScaleOnHover = false;
                utilityButtonScaleOnHover.shouldScaleOnHover = false;

                attackButtonComponent.interactable = false;
                defenseButtonComponent.interactable = false;
                utilityButtonComponent.interactable = false;

                acceptButton.SetActive(false);
            }
        }

        IEnumerator MakeButtonComponentsInteractable()
        {
            yield return new WaitForSeconds(0.1f);

            attackButtonComponent.interactable = true;
            defenseButtonComponent.interactable = true;
            utilityButtonComponent.interactable = true;
        }

        public void EnableAcceptButton()
        {
            acceptButtonComponent.interactable = true;
            acceptButtonScaleOnHover.shouldScaleOnHover = true;

            acceptButtonText.color = enabledTextColor;
        }
    }
}
