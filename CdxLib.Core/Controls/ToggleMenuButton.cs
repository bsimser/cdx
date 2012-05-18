namespace CdxLib.Core.Controls
{
    /// <summary>
    /// A special button that handles toggling between "On" and "Off"
    /// </summary>
    public class ToggleMenuButton : MenuButton
    {
        private readonly string _option;
        private bool _value;

        /// <summary>
        /// Creates a new ToggleMenuButton.
        /// </summary>
        /// <param name="option">The string text to display for the option.</param>
        /// <param name="value">The initial value of the button.</param>
        public ToggleMenuButton(string option, bool value)
            : base(option)
        {
            _option = option;
            _value = value;

            GenerateText();
        }

        /// <summary>
        /// </summary>
        protected override void OnTapped()
        {
            // When tapped we need to toggle the value and regenerate the text
            _value = !_value;
            GenerateText();
            base.OnTapped();
        }

        /// <summary>
        /// Helper that generates the actual Text value the base class uses for drawing.
        /// </summary>
        private void GenerateText()
        {
            Text = string.Format("{0}: {1}", _option, _value ? "On" : "Off");
        }
    }
}