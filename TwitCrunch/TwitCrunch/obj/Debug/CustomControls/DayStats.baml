   M S B A M L   `   `   `  ����Y  UWPFVisifire.Charts, Version=5.1.3.0, Culture=neutral, PublicKeyToken=0cd785983078370bM9clr-namespace:Visifire.Charts;assembly=WPFVisifire.ChartsVisifire.Charts  5       
TwitCrunch(   "TwitCrunch.CustomControls.DayStats   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Hvc9clr-namespace:Visifire.Charts;assembly=WPFVisifire.Charts     5      .��5
      � .+�5   
      Visifire.Charts.Chart  -   5           Name$  chTrend��5         ScrollingEnabled$ falseX�6$      ShadowEnabled .     ZoomingEnabled .    ZoomingMode$ 
MouseWheel=�5   =      PanningMode$ ScrollBarAndMouseDrag=�6V      Titles 5         Visifire.Charts.Title     Text$ Verloop��6      Padding$ 0,10,0,0q�6*   	   FontSize$	 14Q�6=   6M   
   Series
 5          Visifire.Charts.DataSeries     
XValueType$
 Date=�6       RenderAs$ Spline=�62      LineThickness$ 2X�6D   6Y      AxesX 5         Visifire.Charts.Axis     ValueFormatString$ 
dd/MM/yyyy��6      IntervalType$
 Days=�6:      
AxisLabels 5          Visifire.Charts.AxisLabels  5         Angle$	 -90X�6(   66   5      5      5      5      