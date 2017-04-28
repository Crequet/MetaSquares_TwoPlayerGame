using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MetaSquares_TwoPlayerGame
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		CellCollectionCollection cellcollectioncollection;


		public MainWindow()
		{
			cellcollectioncollection = new CellCollectionCollection();
			cellcollectioncollection.BeginUpdate();
			InitializeComponent();
			cellcollectioncollection.EndUpdate();

			cellCollectionCollectionItems.ItemsSource = cellcollectioncollection.CellCollections;
			squareCollection1Items.ItemsSource = cellcollectioncollection.SuqareCollection1.Squares;
			squareCollection2Items.ItemsSource = cellcollectioncollection.SuqareCollection2.Squares;
			playerOneScoreTextBlock.DataContext = cellcollectioncollection;
			playerTwoScoreTextBlock.DataContext = cellcollectioncollection;

		}
		private void rowsText_TextChanged(object sender, TextChangedEventArgs e)
		{
			int value;
			int.TryParse(rowsText.Text, out value);
			cellcollectioncollection.RowCount = value;
			rowsText.Text = cellcollectioncollection.RowCount.ToString();
			rowsText.CaretIndex = rowsText.Text.Length;
		}
		private void columnsText_TextChanged(object sender, TextChangedEventArgs e)
		{
			int value;
			int.TryParse(columnsText.Text, out value);
			cellcollectioncollection.ColumnCount = value;
			columnsText.Text = cellcollectioncollection.ColumnCount.ToString();
			columnsText.CaretIndex = columnsText.Text.Length;
		}
		private void rowsText_columnText_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			int value;
			if (int.TryParse((sender as TextBox).Text, out value))
			{
				value += (e.Delta / 120);
			}
			(sender as TextBox).Text = value.ToString();
		}
		private void cell_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Ellipse cell = sender as Ellipse;
			int i = Convert.ToInt32(cell.Tag.ToString()[0].ToString() + cell.Tag.ToString()[1].ToString());
			int j = Convert.ToInt32(cell.Tag.ToString()[3].ToString() + cell.Tag.ToString()[4].ToString());

			if (e.ChangedButton == MouseButton.Left)
				cellcollectioncollection.CellCollections[i].Cells[j].CellValue++;
			else if (e.ChangedButton == MouseButton.Right)
				cellcollectioncollection.CellCollections[i].Cells[j].CellValue--;
			else if (e.ChangedButton == MouseButton.Middle)
				cellcollectioncollection.CellCollections[i].Cells[j].CellValue = 0;

			cellcollectioncollection.FindSquares();
		}

		public class CellCollectionCollection : INotifyPropertyChanged
		{
			SquareCollection _squarecollection1, _squarecollection2;
			int _rowCount, _columnCount;
			bool _updateactive;
			ObservableCollection<CellCollection> _cellcollections;
			public CellCollectionCollection()
			{
				_rowCount = 1;
				_columnCount = 1;
				_updateactive = true;
				_squarecollection1 = new SquareCollection();
				_squarecollection2 = new SquareCollection();
				CellCollections = new ObservableCollection<CellCollection>();
			}
			public ObservableCollection<CellCollection> CellCollections
			{
				get
				{
					return _cellcollections;
				}
				set
				{
					_cellcollections = value;
				}
			}
			public SquareCollection SuqareCollection1
			{
				get
				{
					return _squarecollection1;
				}
			}
			public SquareCollection SuqareCollection2
			{
				get
				{
					return _squarecollection2;
				}
			}
			public int RowCount
			{
				get
				{
					return _rowCount;
				}
				set
				{
					if (value < 1)
						_rowCount = 1;
					else if (value > 20)
						_rowCount = 20;
					else
						_rowCount = value;
					if (_updateactive) updatecollection();
				}
			}
			public int ColumnCount
			{
				get
				{
					return _columnCount;
				}
				set
				{
					if (value < 1)
						_columnCount = 1;
					else if (value > 20)
						_columnCount = 20;
					else
						_columnCount = value;
					if (_updateactive) updatecollection();
				}
			}
			void updatecollection()
			{
				_squarecollection1.Squares.Clear();
				_squarecollection2.Squares.Clear();
				CellCollections.Clear();
				ObservableCollection<Cell> newcollection;
				for (int i = 0; i < RowCount; i++)
				{
					newcollection = new ObservableCollection<Cell>();
					for (int j = 0; j < ColumnCount; j++)
					{
						newcollection.Add(new Cell(String.Format("{0:d2}-{1:d2}", i, j)));
					}
					CellCollections.Add(new CellCollection() { Cells = newcollection });
				}

				OnPropertyChanged("PlayerOneScore");
				OnPropertyChanged("PlayerTwoScore");
			}
			public void BeginUpdate()
			{
				_updateactive = false;
			}
			public void EndUpdate()
			{
				_updateactive = true;
				updatecollection();
			}
			public void FindSquares()
			{
				int i1, j1, i2, j2, di, dj;
				Square newsquare;
				_squarecollection1.Squares.Clear();
				_squarecollection2.Squares.Clear();
				bool skip;
				List<Square> alreadycheckedsquaresmustskip = new List<Square>();

				for (int s1 = 0; s1 < (RowCount * ColumnCount) - 1; s1++)
				{
					for (int s2 = s1 + 1; s2 < RowCount * ColumnCount; s2++)
					{
						i1 = s1 / (RowCount);
						j1 = s1 % (RowCount);
						i2 = s2 / (RowCount);
						j2 = s2 % (RowCount);
						di = i2 - i1;
						dj = j2 - j1;

						if ((GetCellValueAt(i1, j1) < 1) || (GetCellValueAt(i2, j2) < 1) || (GetCellValueAt(i1, j1) != GetCellValueAt(i2, j2))) continue;

						if (GetCellValueAt(i1, j1) == GetCellValueAt(i1 + di + dj, j1 + dj - di) &&
							GetCellValueAt(i1, j1) == GetCellValueAt(i1 + dj, j1 - di))
						{
							newsquare = new Square(new Point(i1, j1), new Point(di, dj), false);
							skip = false;

							for (int c = 0; c < alreadycheckedsquaresmustskip.Count; c++)
							{
								if (newsquare.Equals(alreadycheckedsquaresmustskip[c]))
								{
									skip = true;
									alreadycheckedsquaresmustskip.RemoveAt(c);
									break;
								}
							}

							if (skip) continue;

							alreadycheckedsquaresmustskip.Add(newsquare);
							newsquare.GeneratePointAndSides();
							if (GetCellValueAt(i1, j1) == 1)
							{
								_squarecollection1.Squares.Add(newsquare);
							}
							else if (GetCellValueAt(i1, j1) == 2)
							{
								_squarecollection2.Squares.Add(newsquare);
							}
						}
					}
				}

				OnPropertyChanged("PlayerOneScore");
				OnPropertyChanged("PlayerTwoScore");
			}
			public int GetCellValueAt(int i, int j)
			{
				if (j >= CellCollections.Count || j < 0)
				{
					return -1;
				}
				else if (i >= CellCollections[j].Cells.Count || i < 0)
				{
					return -1;
				}
				else
				{
					return CellCollections[j].Cells[i].CellValue;
				}
			}
			public double PlayerOneScore
			{
				get
				{
					return SuqareCollection1.Squares.Sum(new Func<Square, double>(square => square.Score));
				}
			}
			public double PlayerTwoScore
			{
				get
				{
					return SuqareCollection2.Squares.Sum(new Func<Square, double>(square => square.Score));
				}
			}
			protected void OnPropertyChanged([CallerMemberName] string PropertyName = null)
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null)
				{
					handler.Invoke(this, new PropertyChangedEventArgs(PropertyName));
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;
		}
		public class CellCollection
		{
			ObservableCollection<Cell> _cells;
			public CellCollection()
			{
				Cells = new ObservableCollection<Cell>();
			}
			public ObservableCollection<Cell> Cells
			{
				get
				{
					return _cells;
				}
				set
				{
					_cells = value;
				}
			}
		}
		public class Cell : INotifyPropertyChanged
		{
			bool _active; bool _cellvalue;
			public Cell(string ID)
			{
				_active = false;
				_cellvalue = false;
				this.ID = ID;
			}
			public Brush CellColor
			{
				get
				{
					if (_active == false)
					{
						return new SolidColorBrush(Color.FromRgb(255, 255, 255));
					}
					else
					{
						if (_cellvalue == false)
						{
							return new RadialGradientBrush(Color.FromRgb(0, 168, 255), Color.FromRgb(0, 74, 112)) { Center = new Point(0.4, 0.4) };
						}
						else
						{
							return new RadialGradientBrush(Color.FromRgb(0, 255, 174), Color.FromRgb(0, 151, 103)) { Center = new Point(0.6, 0.6) };
						}
					}
				}
			}
			public int CellValue
			{
				get
				{
					if (_active == false)
						return 0;
					else if (_cellvalue == false)
						return 1;
					else
						return 2;
				}
				set
				{
					if (value % 3 == 0)
					{
						_active = false;
						_cellvalue = false;
					}
					else if (value % 3 == 1)
					{
						_active = true;
						_cellvalue = false;
					}
					else
					{
						_active = true;
						_cellvalue = true;
					}
					OnPropertyChanged("CellColor");
				}
			}
			public string ID { get; set; }
			protected void OnPropertyChanged([CallerMemberName] string PropertyName = null)
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null)
				{
					handler.Invoke(this, new PropertyChangedEventArgs(PropertyName));
				}
			}
			public event PropertyChangedEventHandler PropertyChanged;
		}
		public class SquareCollection
		{
			public SquareCollection()
			{
				Squares = new ObservableCollection<Square>();
			}
			public ObservableCollection<Square> Squares
			{
				get; set;
			}
		}
		public class Square
		{
			Point _dif, _startingpoint;
			ObservableCollection<Point> _points;
			ObservableCollection<SLine> _sides;
			bool _updateactive;
			public Square(Point StartingPoint, Point Difference, bool AutoGeneratePointAndSides = true)
			{
				_sides = new ObservableCollection<SLine>();
				_points = new ObservableCollection<Point>();

				if (AutoGeneratePointAndSides)
				{
					BeginUpdate();
					this.StartingPoint = StartingPoint;
					this.Difference = Difference;
					EndUpdate();
				}
				else
				{
					this.StartingPoint = StartingPoint;
					this.Difference = Difference;
				}

			}
			public Point StartingPoint
			{
				get
				{
					return _startingpoint;
				}
				set
				{
					_startingpoint = value;
					if (_updateactive)
						updatepointsandsides();
				}
			}
			public Point Difference
			{
				get
				{
					return _dif;
				}
				set
				{
					_dif = value;
					if (_updateactive)
						updatepointsandsides();
				}
			}
			public ObservableCollection<Point> Points
			{
				get
				{
					return _points;
				}
			}
			public ObservableCollection<SLine> Sides
			{
				get
				{
					return _sides;
				}
			}
			public double Score
			{
				get
				{
					return (Difference.X * Difference.X) + (Difference.Y * Difference.Y);
				}
			}
			void updatepointsandsides()
			{
				_points.Clear();
				_points.Add(convertpoint(new Point(StartingPoint.X, StartingPoint.Y)));
				_points.Add(convertpoint(new Point(StartingPoint.X + Difference.X, StartingPoint.Y + Difference.Y)));
				_points.Add(convertpoint(new Point(StartingPoint.X + Difference.X + Difference.Y, StartingPoint.Y + Difference.Y - Difference.X)));
				_points.Add(convertpoint(new Point(StartingPoint.X + Difference.Y, StartingPoint.Y - Difference.X)));

				_sides.Clear();
				_sides.Add(new SLine(_points[0], _points[1]));
				_sides.Add(new SLine(_points[1], _points[2]));
				_sides.Add(new SLine(_points[2], _points[3]));
				_sides.Add(new SLine(_points[3], _points[0]));
			}
			Point convertpoint(Point point)
			{
				return new Point((point.X * 50) + 25, (point.Y * 50) + 25);
			}
			public void BeginUpdate()
			{
				_updateactive = false;
			}
			public void EndUpdate()
			{
				_updateactive = true;
				updatepointsandsides();
			}
			public void GeneratePointAndSides()
			{
				updatepointsandsides();
			}
			public SLine[] GetUnrealSLines()
			{
				SLine[] output = new SLine[4];
				output[0] = new SLine(new Point(StartingPoint.X, StartingPoint.Y), new Point(StartingPoint.X + Difference.X, StartingPoint.Y + Difference.Y));
				output[1] = new SLine(new Point(StartingPoint.X + Difference.X, StartingPoint.Y + Difference.Y), new Point(StartingPoint.X + Difference.X + Difference.Y, StartingPoint.Y + Difference.Y - Difference.X));
				output[2] = new SLine(new Point(StartingPoint.X + Difference.X + Difference.Y, StartingPoint.Y + Difference.Y - Difference.X), new Point(StartingPoint.X + Difference.Y, StartingPoint.Y - Difference.X));
				output[3] = new SLine(new Point(StartingPoint.X + Difference.Y, StartingPoint.Y - Difference.X), new Point(StartingPoint.X, StartingPoint.Y));
				return output;
			}
			public bool Equals(Square square)
			{
				SLine[] squareslines = square.GetUnrealSLines();
				SLine[] thieslines = GetUnrealSLines();
				return ((thieslines[0].Equals(squareslines[0]) && thieslines[1].Equals(squareslines[1]) && thieslines[2].Equals(squareslines[2]) && thieslines[3].Equals(squareslines[3])) ||
						(thieslines[0].Equals(squareslines[1]) && thieslines[1].Equals(squareslines[2]) && thieslines[2].Equals(squareslines[3]) && thieslines[3].Equals(squareslines[0])) ||
						(thieslines[0].Equals(squareslines[2]) && thieslines[1].Equals(squareslines[3]) && thieslines[2].Equals(squareslines[0]) && thieslines[3].Equals(squareslines[1])) ||
						(thieslines[0].Equals(squareslines[3]) && thieslines[1].Equals(squareslines[0]) && thieslines[2].Equals(squareslines[1]) && thieslines[3].Equals(squareslines[2])));
			}
		}

		public class SLine
		{
			public SLine(Point Point1, Point Point2)
			{
				this.Point1 = Point1;
				this.Point2 = Point2;
			}
			public Point Point1 { get; set; }
			public Point Point2 { get; set; }
			public bool Equals(SLine newsline)
			{
				return ((Point1.X == newsline.Point1.X) && (Point1.Y == newsline.Point1.Y) && (Point2.X == newsline.Point2.X) && (Point2.Y == newsline.Point2.Y)) ||
						((Point1.X == newsline.Point2.X) && (Point1.Y == newsline.Point2.Y) && (Point2.X == newsline.Point1.X) && (Point1.Y == newsline.Point2.Y));
			}
		}
	}
}
