   M S B A M L   `   `   `  ����B  >TestApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:TestAppTestApp  5   	   F BDrawNet_WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullV:clr-namespace:DrawNet_WPF.Resizeables;assembly=DrawNet_WPFDrawNet_WPF.Resizeables 5   	       TestApp.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:TestApp     5   	   RResizeables:clr-namespace:DrawNet_WPF.Resizeables;assembly=DrawNet_WPF     5   	     � Title$  
MainWindow��5
   	   $	��450��6   $	��800��6)   .��5      �� -   5         ��Name$ outer��5      .+�5   
   +  %DrawNet_WPF.Resizeables.ResizeWrapper  -   5          Name$ a��5   $   $	��150��6/   $	��150��6;      StrokeThickness$	 5.0Q�6H      Position,  &DrawNet_WPF.Converters.VectorConverter$ 20,20 6^      HandleShape$ Square=�6o      Stroke
 �  ��   AllowResize$ False��6�   .+�5      �� -   5         ��Name$ a1��5      
���  ��	 J� Opacity$		 0.2Q�6/   �� 5      -   5        $ a2��5      
��� � �$		 0.4Q�61   �� 5      -   5        $ a3��5      
����� �$		 0.4Q�60   �� 5      -   5        $ a4��5      
�������$		 0.4Q�65     5      -   5        $ b��5   (   $��50��63   $��50��6>   $	 2.0Q�6J   $ 10,10 6`   
 � � �.+�5      �� -   5        $ a7��5      
����  �$		 0.4Q�64   5      5        5   
   -	   5        $ t��5   $   $	��150��6/   $	��150��6;   $	 5.0Q�6H   $ 100,40 6^   $ Circle=�6p   
 � ���$ False��6�     5   
   -
   5        $ t1��5   $   $	��150��60   $	��150��6<   $	 5.0Q�6I   $ 200,80 6_   $ Circle=�6q   
 � ���$ False��6�   5      5      