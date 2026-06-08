using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    [Header("Background Setup")]
    [SerializeField] Image shopBackgroundImage;
    [SerializeField] Sprite[] entryBackgrounds;
    [SerializeField] Sprite[] noSaleBackgrounds;

    [Header("Shop Buttons")]
    [SerializeField] Button btnPie;
    [SerializeField] Button btnIceCream;
    [SerializeField] Button btnGarryBowl;
    [SerializeField] Button btnSeaUrchin;
    [SerializeField] Button btnSecretFormula;
    [SerializeField] Button btnLeave;

    [Header("Price Text Labels")]
    [SerializeField] TMP_Text textPiePrice;
    [SerializeField] TMP_Text textIceCreamPrice;
    [SerializeField] TMP_Text textGarryBowlPrice;
    [SerializeField] TMP_Text textSeaUrchinPrice;
    [SerializeField] TMP_Text textSecretFormulaPrice;

    [Header("Arcade Menu Settings")]
    [SerializeField] float focusScaleModifier = 1.1f;
    [SerializeField] float scaleSpeed = 12f;

    int pricePie, priceIceCream, priceGarryBowl, priceSeaUrchin, priceSecretFormula;

    int activeBackgroundIndex = 0;
    bool madeAnyPurchase = false;
    bool isTransitioning = false;

    void OnEnable()
    {
        madeAnyPurchase = false;
        isTransitioning = false;
        if (btnLeave != null) btnLeave.interactable = true;

        SetupBackground();
        SetupInventoryButtons();

        StartCoroutine(InitializeArcadeNavigationRoutine());
    }

    void Update()
    {
        HandleArcadeActionInput();
        HandleArcadeSelectionEffects();
    }

    IEnumerator InitializeArcadeNavigationRoutine()
    {
        yield return new WaitForEndOfFrame();
        RebuildArcadeNavigation();
        FocusFirstAvailableButton();
    }

    void HandleArcadeActionInput()
    {
        if (isTransitioning) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
            {
                Button targetedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                
                if (targetedButton != null && targetedButton.interactable)
                {
                    targetedButton.onClick.Invoke();
                }
            }
        }
    }

    void RebuildArcadeNavigation()
    {
        List<Button> availableButtons = new List<Button>();

        if (btnPie != null && btnPie.gameObject.activeSelf) availableButtons.Add(btnPie);
        if (btnIceCream != null && btnIceCream.gameObject.activeSelf) availableButtons.Add(btnIceCream);
        if (btnGarryBowl != null && btnGarryBowl.gameObject.activeSelf) availableButtons.Add(btnGarryBowl);
        if (btnSeaUrchin != null && btnSeaUrchin.gameObject.activeSelf) availableButtons.Add(btnSeaUrchin);
        if (btnSecretFormula != null && btnSecretFormula.gameObject.activeSelf) availableButtons.Add(btnSecretFormula);

        if (btnLeave != null && btnLeave.gameObject.activeSelf) availableButtons.Add(btnLeave);

        if (availableButtons.Count == 0) return;

        for (int i = 0; i < availableButtons.Count; i++)
        {
            Navigation nav = new Navigation { mode = Navigation.Mode.Explicit };

            int nextIndex = (i + 1) % availableButtons.Count;
            int prevIndex = (i - 1 + availableButtons.Count) % availableButtons.Count;

            nav.selectOnRight = availableButtons[nextIndex];
            nav.selectOnDown = availableButtons[nextIndex];
            nav.selectOnLeft = availableButtons[prevIndex];
            nav.selectOnUp = availableButtons[prevIndex];

            availableButtons[i].navigation = nav;
        }
    }

    void FocusFirstAvailableButton()
    {
        if (EventSystem.current == null) return;

        Button[] orderedButtons = { btnPie, btnIceCream, btnGarryBowl, btnSeaUrchin, btnSecretFormula, btnLeave };

        foreach (var btn in orderedButtons)
        {
            if (btn != null && btn.gameObject.activeSelf && btn.interactable)
            {
                EventSystem.current.SetSelectedGameObject(btn.gameObject);
                return;
            }
        }
    }

    void HandleArcadeSelectionEffects()
    {
        if (EventSystem.current == null) return;

        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected == null)
        {
            FocusFirstAvailableButton();
            return;
        }

        Button[] allButtons = { btnPie, btnIceCream, btnGarryBowl, btnSeaUrchin, btnSecretFormula, btnLeave };

        foreach (var btn in allButtons)
        {
            if (btn == null) continue;

            bool isHighlighted = currentSelected == btn.gameObject;
            Vector3 targetScale = isHighlighted ? Vector3.one * focusScaleModifier : Vector3.one;

            btn.transform.localScale = Vector3.Lerp(btn.transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }
    }

    void SetupBackground()
    {
        if (entryBackgrounds.Length == 0 || shopBackgroundImage == null) return;

        activeBackgroundIndex = Random.Range(0, entryBackgrounds.Length);
        shopBackgroundImage.sprite = entryBackgrounds[activeBackgroundIndex];
    }

    void SetupInventoryButtons()
    {
        CalculateRandomPrices();
        SetPrices();
        EnableButtons();
        RegisterButtonClicks();
    }

    void CalculateRandomPrices()
    {
        pricePie = GetRandomPrice(100, 350);
        priceIceCream = GetRandomPrice(200, 450);
        priceGarryBowl = GetRandomPrice(50, 180);
        priceSeaUrchin = GetRandomPrice(70, 240);
        priceSecretFormula = GetRandomPrice(120, 300);
    }

    int GetRandomPrice(int minimum, int maximum)
    {
        return Random.Range(minimum, maximum);
    }

    void SetPrices()
    {
        SetPrice(textPiePrice, pricePie);
        SetPrice(textIceCreamPrice, priceIceCream);
        SetPrice(textGarryBowlPrice, priceGarryBowl);
        SetPrice(textSeaUrchinPrice, priceSeaUrchin);
        SetPrice(textSecretFormulaPrice, priceSecretFormula);
    }

    void SetPrice(TMP_Text priceText, int price)
    {
        if (priceText == null) return;
        priceText.text = $"${price}";
    }

    void EnableButtons()
    {
        EnableButton(btnPie);
        EnableButton(btnIceCream);
        EnableButton(btnGarryBowl);
        EnableButton(btnSeaUrchin);
        EnableButton(btnSecretFormula);
    }

    void EnableButton(Button buttonToEnable)
    {
        if (buttonToEnable == null) return;
        buttonToEnable.gameObject.SetActive(Random.value > 0.5f);
    }

    void RegisterButtonClicks()
    {
        ConfigureButton(btnPie, pricePie, "Pie");
        ConfigureButton(btnIceCream, priceIceCream, "IceCream");
        ConfigureButton(btnGarryBowl, priceGarryBowl, "GarryBowl");
        ConfigureButton(btnSeaUrchin, priceSeaUrchin, "SeaUrchin");
        ConfigureButton(btnSecretFormula, priceSecretFormula, "SecretFormula");

        if (btnLeave != null)
        {
            btnLeave.onClick.RemoveAllListeners();
            btnLeave.onClick.AddListener(HandleLeaveButton);
        }
    }

    void ConfigureButton(Button button, int price, string itemID)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => TryPurchaseItem(price, button, itemID));
    }

    void TryPurchaseItem(int itemPrice, Button clickedButton, string itemID)
    {
        if (GameManager.Instance.CurrentMoney >= itemPrice)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayPurchase();

            GameManager.Instance.SpendMoney(itemPrice);
            madeAnyPurchase = true;

            ApplyItemEffect(itemID);

            clickedButton.gameObject.SetActive(false);

            RebuildArcadeNavigation();
            FocusFirstAvailableButton();
        }
        else
        {
            Debug.Log($"Not enough money to purchase {itemID}");
        }
    }

    void ApplyItemEffect(string itemID)
    {
        Hook playerHook = FindFirstObjectByType<Hook>();

        switch (itemID)
        {
            case "Pie":
                GameManager.Instance.BuyExplosivePie();
                break;
            case "IceCream":
                GameManager.Instance.SetStrengthBuff(true);
                break;
            case "GarryBowl":
                GameManager.Instance.SetGarryBowlBuff(true);
                break;
            case "SeaUrchin":
                GameManager.Instance.SetSeaUrchinBuff(true);
                break;
            case "SecretFormula":
                GameManager.Instance.SetSecretFormulaBuff(true);
                break;
        }
    }

    void HandleLeaveButton()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        if (btnLeave != null) btnLeave.interactable = false;

        if (madeAnyPurchase)
        {
            ExitShopPhase();
        }
        else
        {
            StartCoroutine(SquidwardDisappointmentSequence());
        }
    }

    IEnumerator SquidwardDisappointmentSequence()
    {
        if (noSaleBackgrounds.Length > activeBackgroundIndex && shopBackgroundImage != null)
        {
            shopBackgroundImage.sprite = noSaleBackgrounds[activeBackgroundIndex];
        }

        yield return new WaitForSeconds(1.5f);
        ExitShopPhase();
    }

    void ExitShopPhase()
    {
        GameManager.Instance.StartNextLevel();
    }
}
