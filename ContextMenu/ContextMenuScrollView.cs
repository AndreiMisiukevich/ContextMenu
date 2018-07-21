using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContextMenu
{
	public enum ScrollDirection { Close, Open }
	public enum ScrollState { Closed, Opened, Moving }

	public class ContextMenuScrollView : ScrollView
	{
		public event Action TouchStarted;
		public event Action ActionBarOpened;

		private View _contentView;
		private View _contextView = new ContentView { WidthRequest = 1 };

		public ContextMenuScrollView()
		{
			Orientation = ScrollOrientation.Horizontal;
			VerticalOptions = LayoutOptions.Fill;
			HorizontalOptions = LayoutOptions.Fill;

			Content = ViewStack = new StackLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Spacing = 0,
				Orientation = StackOrientation.Horizontal,
				Children = {
					_contextView
				}
			};

			Scrolled += OnScrolled;
		}

		protected StackLayout ViewStack { get; }
		protected ScrollDirection CurrentDirection { get; private set; }
		protected ScrollState CurrentState { get; private set; }
		protected bool HasBeenAccelerated { get; private set; }
		protected double PrevScrollX { get; private set; }
		protected bool IsInteracted { get; private set; }
		protected bool IsOpenDirection => CurrentDirection == ScrollDirection.Open;
		protected bool IsDirectionAndStateSame => (int)CurrentDirection == (int)CurrentState;

		public View ContentView
		{
			get => _contentView;
			set
			{
				if (value != _contentView)
				{
					if (_contentView != null)
					{
						ViewStack.Children.Remove(_contentView);
					}
					if (value != null)
					{
						ViewStack.Children.Insert(0, value);
					}
				}
				_contentView = value;
			}
		}

		public View ContextView
		{
			get => _contextView;
			set
			{
				value = value ?? new ContentView { WidthRequest = 1 };
				if (value != _contextView)
				{
					value.IsVisible = false;
					ViewStack.Children.Remove(_contextView);
					ViewStack.Children.Add(value);
					value.IsVisible = true;
				}
				_contextView = value;
			}
		}

		public async void ForceCloseContextMenu(ContextMenuScrollView view, bool animated)
		{
			if (view == null)
			{
				return;
			}

			try
			{
				if (view.ScrollX > 0)
				{
					await view.ScrollToAsync(0, 0, animated);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public virtual void OnTouchStarted()
		{
			IsInteracted = true;
			TouchStarted?.Invoke();
			HasBeenAccelerated = false;
		}

		public virtual async void OnTouchEnded()
		{
			IsInteracted = false;
			CheckActionBarOpened();
			if (Device.RuntimePlatform == Device.Android)
			{
				await Task.Delay(10);
			}

			if (HasBeenAccelerated)
			{
				return;
			}

			var width = ContextView.Width;
			var isOpen = IsOpenDirection
						? ScrollX > GetMovingWidth(width)
						: ScrollX > width - GetMovingWidth(width);
			await ScrollToAsync(isOpen ? width : 0, 0, true);
		}

		public virtual async Task OnFlingStarted(bool needScroll = true, bool animated = true, bool inMainThread = false)
		{
			HasBeenAccelerated = true;

			if (needScroll)
			{
				var task = ScrollToAsync(IsOpenDirection ? ContextView.Width : 0, 0, animated);
				if (inMainThread)
				{
					var completionSource = new TaskCompletionSource<bool>();
					Device.BeginInvokeOnMainThread(async () =>
					{
						await task;
						completionSource.SetResult(true);
					});
					await completionSource.Task;
					return;
				}
				await task;
			}
		}

		public async Task MoveSideMenu(bool isOpen = false, bool animated = true)
		{
			CurrentDirection = isOpen
				? ScrollDirection.Open
				: ScrollDirection.Close;

			await OnFlingStarted(!IsDirectionAndStateSame, animated, true);
		}

		protected async void OnMoveActionInvoked(bool isOpen = false)
		{
			await MoveSideMenu(isOpen, true);
		}

		protected virtual void OnScrolled(object sender, ScrolledEventArgs args)
		{
			CurrentDirection = Math.Abs(PrevScrollX - ScrollX) < double.Epsilon
					? CurrentDirection
					: PrevScrollX > ScrollX
						? ScrollDirection.Close
						: ScrollDirection.Open;
			PrevScrollX = ScrollX;

			CheckScrollState();
			CheckActionBarOpened();
		}

		protected virtual double GetMovingWidth(double contextWidth)
		=> contextWidth * 0.33;

		protected void CheckScrollState()
		{
			if (Math.Abs(ScrollX) <= double.Epsilon)
			{
				CurrentState = ScrollState.Closed;
				return;
			}

			if (Math.Abs(ScrollX - ContextView.Width) <= double.Epsilon)
			{
				CurrentState = ScrollState.Opened;
				return;
			}

			CurrentState = ScrollState.Moving;
		}

		protected bool CheckIsOpen()
		{
			var width = ContextView.Width;
			return IsOpenDirection
							? ScrollX > GetMovingWidth(width)
							: ScrollX > width - GetMovingWidth(width);
		}

		protected void CheckActionBarOpened()
		{
			if(ScrollX >= ContextView.Width && !IsInteracted)
			{
				ActionBarOpened?.Invoke();
			}
		}
	}
}
