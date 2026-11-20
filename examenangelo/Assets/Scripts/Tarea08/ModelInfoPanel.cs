using UnityEngine;
using UnityEngine.UI;

namespace LolSuperman.Tarea08
{
    public sealed class ModelInfoPanel : MonoBehaviour
    {
        [Header("Información del modelo")]
        [SerializeField] private Text modelNameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text categoryText;
        [SerializeField] private Text levelText;
        [SerializeField] private Image modelIcon;
        [SerializeField] private GameObject emptyIcon;
        [SerializeField] private GameObject fichaDesplegable;
        [Header("Estado del seguimiento")]
        [SerializeField] private Text trackingText;
        [SerializeField] private Text instructionText;

        public const string OriginalName = "Selecciona un modelo";
        public const string OriginalDescription = "Apunta a una imagen de referencia.\nDespués, toca su modelo para consultar la ficha.";
        public const string OriginalCategory = "Sin seleccionar";
        public const string OriginalLevel = "—";

        public ModelInfo CurrentInformation { get; private set; }
        public string DisplayedName => modelNameText != null ? modelNameText.text : string.Empty;
        public string DisplayedDescription => descriptionText != null ? descriptionText.text : string.Empty;
        public string DisplayedCategory => categoryText != null ? categoryText.text : string.Empty;
        public string DisplayedLevel => levelText != null ? levelText.text : string.Empty;
        public Sprite DisplayedIcon => modelIcon != null ? modelIcon.sprite : null;

        public void Configure(Text title, Text description, Text category, Text level,
            Image icon, GameObject placeholder, Text tracking, Text instruction)
        {
            modelNameText = title;
            descriptionText = description;
            categoryText = category;
            levelText = level;
            modelIcon = icon;
            emptyIcon = placeholder;
            trackingText = tracking;
            instructionText = instruction;
            ResetToOriginal();
            SetTrackedCount(0);
        }

        private void Awake()
        {
            ResetToOriginal();
            SetTrackedCount(0);
        }

        public void Show(ModelInfo information)
        {
            if (information == null) { ResetToOriginal(); return; }
            CurrentInformation = information;
            ShowValues(information.ModelName, information.Description, information.Category, information.Level, information.Icon);
        }

        public void Show(ElementData creature)
        {
            if (creature == null) { ResetToOriginal(); return; }
            CurrentInformation = null;
            ShowValues(creature.elementName, creature.description, creature.type, creature.level, creature.icon);
        }

        private void ShowValues(string name, string description, string type, int level, Sprite icon)
        {
            SetText(modelNameText, name);
            SetText(descriptionText, description);
            SetText(categoryText, type);
            SetText(levelText, level.ToString());
            if (modelIcon != null)
            {
                modelIcon.sprite = icon;
                modelIcon.gameObject.SetActive(icon != null);
            }
            if (emptyIcon != null) emptyIcon.SetActive(icon == null);
            if (fichaDesplegable != null) fichaDesplegable.SetActive(true);
        }

        public void ResetToOriginal()
        {
            CurrentInformation = null;
            SetText(modelNameText, OriginalName);
            SetText(descriptionText, OriginalDescription);
            SetText(categoryText, OriginalCategory);
            SetText(levelText, OriginalLevel);
            if (modelIcon != null)
            {
                modelIcon.sprite = null;
                modelIcon.gameObject.SetActive(false);
            }
            if (emptyIcon != null) emptyIcon.SetActive(true);
            if (fichaDesplegable != null) fichaDesplegable.SetActive(false);
        }

        public void SetTrackedCount(int count)
        {
            SetText(trackingText, count == 0 ? "BUSCANDO IMÁGENES" :
                count == 1 ? "1 MODELO DETECTADO" : count + " MODELOS DETECTADOS");
            if (fichaDesplegable != null)
                SetText(instructionText, count == 0 ? "Apunta a una de las cuatro tarjetas" : "Toca una criatura para consultar su ficha");
            else
                SetText(instructionText, count == 0 ? "Apunta a Greninja, Paletas o Miku" : "Toca un modelo para ver su información");
        }

        private static void SetText(Text target, string value)
        {
            if (target != null) target.text = value;
        }
    }
}
