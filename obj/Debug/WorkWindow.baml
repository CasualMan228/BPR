   M S B A M L   `   `   `  ����>  :BPR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:BPRBPR  5   	       BPR.WorkWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:BPR     5   	   b ^MaterialDesignThemes.Wpf, Version=3.2.0.1979, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5	   	     � Icon$  app.ico��5   	    � Title$ Основное окно��5
   	   $	��800��6   $
��1150��6,    � WindowStartupLocation$ CenterScreen=�69    � 
ResizeMode$ NoResize=�6^   .��5      � .+�5   
   �� $��background.jpg��6    J� Effect 5      -  'System.Windows.Media.Effects.BlurEffect  5         Radius$ 15Q�6   5      ]� 5   
    K� ZIndex$ 1��6   ��.  .+�5      o� $	��Top=�6   $��30��6(   .@�5      m� -   5        $��	О нас��5      $��30��6@   $��Medium"�6L   m� 5      -   5        $��Справка��5      $��30��6?   $��Medium"�6K   m� 5      -   5         m�Name$ 
adminPanel��5      $��Админ-панель��6/   $��30��6E   $��Medium"�6Q   .@�5      m� -   5        $%��Добавить самолет��5      $��Medium"�6P   m� 5      -   5        $%��Изменить самолет��5      $��Medium"�6Q   m� 5      -   5        $#��Удалить самолет��5      $��Medium"�6R   m� 5      -   5        $M��GУчет бронирований (выставленные счета)��5      $��Medium"�6c   m� 5      -   5        $��Статистика��5      $��Medium"�6L   5      5      5      �� 5    
   $	��680��6   $	��680��6!   $
��1100��61   ���#FFFAFA �� CornerRadius$ 10��6T   $��Center=�6f   $��Center=�6�   ._�5!      � 	 � RowDefinitions	 5"      �� 5#      $��7* �6$   �� 5$      $	��65* �6$   
 � ColumnDefinitions
 5&      �� 5'      �� 5(      �� 5)      .+�5+      �� $��0��6   $��0��6&   ���#C8F7F7$ 10, 0, 0, 0��6K   ._�5,      �� -	   5         ��Name$ filter��5,   M    � Cursor$
 Hand��6   $��0��6]   $��Center=�6m   $
��Left=�6�   $��
filter.png��6�   $��30��6�   $��30��6�   $
��30 0q�6�    �� BitmapScalingMode$ HighQuality=�6�   5-      �� 5.      $��0��6   $��1��6&   ���#C8F7F7._�5/      � $��Center=�6   
 50      �� 51      $��1.25* �6/   �� 52      .+�54      �� -
   5         ��Name$ textBoxPlaneName��54   V   $��0��6"   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$6 0Введите название самолета��6p    %  !MaterialDesignFloatingHintTextBox#��[  6�    �� 
CaretBrush �#191970$��0,4,0,15q�6�   $
��Bold"�6  $��15(�6  $��Center=�6-  �� 55      -   5        $ search��55   0   $��1��6    $��
search.png��6@   $��25��6T   $��25��6_   $
��Left=�6k   $��	15,0,0,-2q�6�   $ HighQuality=�6�   56      57      �� 58      $��0��6   $��2��6&   ���#C8F7F7$ 0, 10, 0, 0��6K   ._�59      � .+�5:      �� -   5        $
 sort��5:   �   $
 Hand��6    $��2��6O   $��Center=�6_   $��Right=�6z   $��sort.png��6�   $��35��6�   $��35��6�   $
��30 0q�6�   $ HighQuality=�6�   5;      5<      �� 5=      -   5         ��Name$ scrollViewer��5=   �   $��1��6   $��3��6,   $
��Auto=�6@   $��Disabled=�6c   .��5>      � -   5         �Name$ contentPanel��5>   9    � Orientation$ 
Horizontal=�6    $��10q�6O   5?      5@      5A      5B      5C      