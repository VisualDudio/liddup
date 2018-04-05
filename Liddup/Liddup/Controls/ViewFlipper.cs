using Liddup.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Liddup.Controls
{
    public class ViewFlipper : ContentView
    {
        public static readonly BindableProperty FrontViewProperty = BindableProperty.Create(nameof(FrontView), typeof(View), typeof(ViewFlipper), propertyChanged: FrontViewChanged);
        public static readonly BindableProperty BackViewProperty = BindableProperty.Create(nameof(BackView), typeof(View), typeof(ViewFlipper), propertyChanged: BackViewChanged);
        public static readonly BindableProperty FlipOnTapProperty = BindableProperty.Create(nameof(FlipOnTap), typeof(bool), typeof(ViewFlipper), true);
        public static readonly BindableProperty FlipStateProperty = BindableProperty.Create(nameof(FlipState), typeof(FlipState), typeof(ViewFlipper), defaultValue: FlipState.Front, propertyChanged: FlipStateChanged);
        public static readonly BindableProperty RotationDirectionProperty = BindableProperty.Create(nameof(RotationDirection), typeof(RotationDirection), typeof(ViewFlipper), RotationDirection.Horizontal);
        public static readonly BindableProperty AnimationDurationProperty = BindableProperty.Create(nameof(AnimationDuration), typeof(int), typeof(ViewFlipper), 250);

        public View FrontView
        {
            get { return (View)GetValue(FrontViewProperty); }
            set { SetValue(FrontViewProperty, value); }
        }
      
        public View BackView
        {
            get { return (View)GetValue(BackViewProperty); }
            set { SetValue(BackViewProperty, value); }
        }
     
        public bool FlipOnTap
        {
            get { return (bool)GetValue(FlipOnTapProperty); }
            set { SetValue(FlipOnTapProperty, value); }
        }
      
        public FlipState FlipState
        {
            get { return (FlipState)GetValue(FlipStateProperty); }
            set { SetValue(FlipStateProperty, value); }
        }
       
        public int AnimationDuration
        {
            get { return (int)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }
      
        public RotationDirection RotationDirection
        {
            get { return (RotationDirection)GetValue(RotationDirectionProperty); }
            set { SetValue(RotationDirectionProperty, value); }
        }

        public ViewFlipper()
        {
            GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(OnTapped), NumberOfTapsRequired = 2 });
        }

        private async void Flip()
        {
            var animationDuration = (uint)Math.Round((double)AnimationDuration / 2);

            if (FlipState == FlipState.Front)
            {
                // Perform half of the flip
                if (RotationDirection == RotationDirection.Horizontal)
                    await this.RotateYTo(90, animationDuration);
                else
                    await this.RotateXTo(90, animationDuration);

                // Change the visible content
                Content = FrontView;

                // Perform second half of the flip
                if (RotationDirection == RotationDirection.Horizontal)
                    await this.RotateYTo(0, animationDuration);
                else
                    await this.RotateXTo(0, animationDuration);
            }
            else
            {
                // Perform half of the flip
                if (RotationDirection == RotationDirection.Horizontal)
                    await this.RotateYTo(90, animationDuration);
                else
                    await this.RotateXTo(90, animationDuration);

                // Change the visible content
                Content = BackView;

                // Perform second half of the flip
                if (RotationDirection == RotationDirection.Horizontal)
                    await this.RotateYTo(180, animationDuration);
                else
                    await this.RotateXTo(180, animationDuration);
            }
        }

        private void SetBackviewRotation()
        {
            if (RotationDirection == RotationDirection.Horizontal)
            {
                BackView.RotationX = 0;
                BackView.RotationY = 180;
            }
            else
            {
                BackView.RotationY = 0;
                BackView.RotationX = 180;
            }
        }

        private async void OnTapped()
        {
            if (!FlipOnTap) return;

            IsEnabled = false;
            FlipState = FlipState == FlipState.Front ? FlipState.Back : FlipState.Front;
            await Task.Delay(500);
            IsEnabled = true;
        }
     
        private static void FlipStateChanged(BindableObject obj, object oldValue, object newValue)
        {
            ViewFlipper flipper = obj as ViewFlipper;
            if (flipper == null) return;

            (flipper.BindingContext as Song).AddToQueueCommand.Execute(null);

            flipper.Flip();
        }

        private static void FrontViewChanged(BindableObject obj, object oldValue, object newValue)
        {
            ViewFlipper flipper = obj as ViewFlipper;
            if (flipper == null) return;

            if (oldValue as View == null)
                flipper.Content = newValue as View;
        }
     
        private static void BackViewChanged(BindableObject obj, object oldValue, object newValue)
        {
            ViewFlipper flipper = obj as ViewFlipper;
            if (flipper == null || newValue as View == null) return;

            flipper.SetBackviewRotation();

        }
    
        private static void RotationDirectionChanged(BindableObject obj, object oldValue, object newValue)
        {
            ViewFlipper flipper = obj as ViewFlipper;
            if (flipper == null || flipper.BackView == null) return;

            flipper.SetBackviewRotation();
        }

    }
 
    public enum FlipState
    {
        Front,
        Back
    }
   
    public enum RotationDirection
    {
        Horizontal,
        Vertical
    }
}
