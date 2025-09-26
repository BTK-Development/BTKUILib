using MelonLoader;
using System;
using ABI_RC.Core;

namespace BTKUILib.UIObjects.Objects
{
    /// <summary>
    /// This object contains all information and setup for multiselections
    /// </summary>
    public class MultiSelection
    {
        internal readonly ABI_RC.Systems.UI.UILib.UIObjects.Objects.MultiSelection InternalMultiSelect;
        
        /// <summary>
        /// Option array
        /// </summary>
        public string[] Options;

        /// <summary>
        /// Action to listen for changes of the selection
        /// </summary>
        public Action<int> OnOptionUpdated;

        /// <summary>
        /// Name of the multiselection
        /// </summary>
        public string Name;

        /// <summary>
        /// Get or set the currently selected index
        /// </summary>
        public int SelectedOption
        {
            get => InternalMultiSelect.SelectedOption;
            set => InternalMultiSelect.SelectedOption = value;
        }

        /// <summary>
        /// Create a new multiselection object
        /// </summary>
        /// <param name="name">Name to be displayed on the multiselection page when opened</param>
        /// <param name="options">Options to be displayed</param>
        /// <param name="selectedOption">Index of currently selected object</param>
        public MultiSelection(string name, string[] options, int selectedOption)
        {
            InternalMultiSelect = new ABI_RC.Systems.UI.UILib.UIObjects.Objects.MultiSelection(name, options, selectedOption);
            InternalMultiSelect.OnOptionUpdated += i =>
            {
                OnOptionUpdated?.Invoke(InternalMultiSelect.SelectedOption);
            };
        }

        /// <summary>
        /// Sets SelectedOption without triggering the OnOptionUpdated
        /// </summary>
        /// <param name="option"></param>
        public void SetSelectedOptionWithoutAction(int option)
        {
            InternalMultiSelect.SetSelectedOptionWithoutAction(option);
        }

        /// <summary>
        /// Generates a MultiSelection object from a MelonPref that uses an Enum
        /// </summary>
        /// <param name="entry">MelonPreferences_Entry to generate MultiSelection for</param>
        /// <typeparam name="TEnum">Enum in use with the MelonPref</typeparam>
        /// <returns>Generated MultiSelection with action and options configured based off the MelonPref and Enum</returns>
        public static MultiSelection CreateMultiSelectionFromMelonPref<TEnum>(MelonPreferences_Entry<TEnum> entry) where TEnum : Enum
        {
            MultiSelection multiSelection = new(
                entry.DisplayName,
                CVRTools.GetPrettyEnumNames<TEnum>(),
                CVRTools.GetEnumIndex(entry.Value)
            )
            {
                OnOptionUpdated = i => entry.Value = (TEnum)Enum.Parse(typeof(TEnum), Enum.GetNames(typeof(TEnum))[i])
            };

            return multiSelection;
        }
    }
}