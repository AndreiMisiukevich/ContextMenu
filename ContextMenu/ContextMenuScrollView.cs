using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContextMenu
{
	public enum GalleyScrollDirection { Close, Open }
	public enum GalleyScrollState { Closed, Opened, Moving }

	public class ContextMenuScrollView : ScrollView
	{
		public event Action TouchStarted;
		public event Action ContextMenuOpened;

		private View _contentView;
		private View _contextView = new ContentView { WidthRequest = 1 };

		public StackLayout ViewStack { get; }
		public GalleyScrollDirection CurrentDirection { get; private set; }
		public GalleyScrollState CurrentState { get; private set; }
		public bool HasBeenAccelerated { get; private set; }
		public double PrevScrollX { get; private set; }

		public bool IsOpenDirection => CurrentDirection == GalleyScrollDirection.Open;
		public bool IsDirectionAndStateSame => (int)CurrentDirection == (int)CurrentState;

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
				if (value != _contextView)
				{
					ViewStack.Children.Remove(_contextView);
					ViewStack.Children.Add(value ?? new ContentView { WidthRequest = 1 });
				}
				_contextView = value;
			}
		}

		public bool IsClosed => CurrentState == GalleyScrollState.Closed;

		public bool IsOneMenuCanBeOpened { get; set; } = true;

		public async void ForceCloseContextMenu(ContextMenuScrollView view)
		{
			if (view == null)
			{
				return;
			}

			try
			{
				if (view.ScrollX > 0)
				{
					var task = view.ScrollToAsync(0, 0, true);
					if (view == null)
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
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public virtual void OnTouchStarted()
		{
			TouchStarted?.Invoke();
			HasBeenAccelerated = false;
		}

		public virtual async void OnTouchEnded()
		{
			if (Device.RuntimePlatform == Device.Android)
			{
				await Task.Delay(10);
			}

			if (HasBeenAccelerated)
			{
				return;
			}

			var width = GetContextViewWidth();
			var isOpen = IsOpenDirection
						? ScrollX > GetMovingWidth(width)
						: ScrollX > width - GetMovingWidth(width);
			await ScrollToAsync(isOpen ? width : 0, 0, true);

			if (!HasBeenAccelerated && CheckIsOpen() && IsOneMenuCanBeOpened)
			{
				ContextMenuOpened?.Invoke();
			}
		}

		public virtual async Task OnFlingStarted(bool needScroll = true, bool animated = true, bool inMainThread = false)
		{
			if (needScroll && IsOneMenuCanBeOpened)
			{
				ContextMenuOpened?.Invoke();
			}

			HasBeenAccelerated = true;
			if (needScroll)
			{

				var task = ScrollToAsync(IsOpenDirection ? GetContextViewWidth() : 0, 0, animated);
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
				? GalleyScrollDirection.Open
				: GalleyScrollDirection.Close;

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
						? GalleyScrollDirection.Close
						: GalleyScrollDirection.Open;
			PrevScrollX = ScrollX;

			CheckScrollState();
		}

		protected virtual void CheckScrollState()
		{
			if (Math.Abs(ScrollX) <= double.Epsilon)
			{
				CurrentState = GalleyScrollState.Closed;
			}
			else if (Math.Abs(ScrollX - GetContextViewWidth()) <= double.Epsilon)
			{
				CurrentState = GalleyScrollState.Opened;
			}
			else
			{
				CurrentState = GalleyScrollState.Moving;
			}
		}

		protected virtual double GetContextViewWidth() 
		=> ContextView.Width;

		protected virtual double GetMovingWidth(double contextWidth)
		=> contextWidth * 0.3;

		protected bool CheckIsOpen()
		{
			var width = GetContextViewWidth();
			return IsOpenDirection
							? ScrollX > GetMovingWidth(width)
							: ScrollX > width - GetMovingWidth(width);
		}
	}
}
