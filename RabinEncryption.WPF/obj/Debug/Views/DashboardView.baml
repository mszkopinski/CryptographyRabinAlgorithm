   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5      N JRabinEncryption.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:RabinEncryption.WPF.ViewsRabinEncryption.WPF.Views 5      -   'RabinEncryption.WPF.Views.DashboardView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5      9local'clr-namespace:RabinEncryption.WPF.Views     5      .��5
      � ���#2c2c2c  � ColumnDefinitions  5   
   �� 5      $	��190 �6   �� 5      $��* �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   .+�5      � $��0��6   $��2��6#    5      �� 5      $��50 �6$   �� 5      $��50 �6$   �� 5      $��20 �6$   �� 5      $��50 �6$   �� 5      $��50 �6$   �� 5      $��20 �6$   �� 5      $��50 �6$   �� 5      $��50 �6$   .+�5      �� $��0��6D   $��0��6Q   ���#232323���#CCCCCC$��20(�5    C   $
��Bold"�6Q   ��5      �� *EncryptMessageCommand+.��5    c   	ENCRYPT5"      �� 5#      $��1��6D   $��0��6Q   ���#232323���#CCCCCC$��20(�5$   C   $
��Bold"�6Q   ��5#      �� *DecryptMessageCommand+{�5$   c   �� *
CanDecrypt+.��6�   	DECRYPT5&      �� 5'      $��2��6   
���   �6)   �� 5(      $��3��6>   $��0��6K   ���#232323���#CCCCCC$��20(�5)   C   $
��Bold"�6Q   ��5(      �� *LoadFileCommand+.��5)   c   	LOAD FILE5+      �� 5,      $��4��6>   $��0��6K   ���#232323���#CCCCCC$��20(�5-   C   $
��Bold"�6Q   ��5,      �� *SaveFileCommand+.��5-   c   	SAVE FILE5/      �� 50      $��5��6   
���   �6)   �� 51      $��6��6A   $��0��6N   ���#232323���#CCCCCC$��20(�52   C   $
��Bold"�6Q   ��51      �� *RefreshKeysCommand+.��52   c   GENERATE KEYS54      �� 55      $��7��6F   $��0��6S   ���#232323���#CCCCCC$��20(�56   C   $
��Bold"�6Q   ��55      �� *RefreshPublicKeyCommand+.��56   c   REFRESH PUBLIC58      59      �� 5T   
   $��1��6   $��2��6%   $��0��69   $��2��6F   $��Center=�6X    �� Orientation$ 
Horizontal=�6s   $
��Left=�6�   .+�5U      �� 
�������$��15(�6'   $��Center=�65   $��	10,0,0,30q�6P   $
��Left=�6c   .��6�   	CONTENT6�   5V      �� 5X   
   $��1��6   $��2��6%   $��0��69   $��2��6F   $��Center=�6X   $ 
Horizontal=�6s   $��Right=�6�   .+�5Y      �� 
�������$��15(�6'   $��Center=�65   $��	10,0,0,30q�6P   $
��Left=�6c   .��6�   	ENCRYPTED6�   5Z      �� 5\   
   $��1��6   $��2��6%   $��0��69   $��2��6F   $��Center=�6X   $ 
Horizontal=�6s   $��Center=�6�   .+�5]      �� 
�������$��15(�6'   $��Center=�65   $��10,0,0,0q�6P   $
��Left=�6b   .��6�   PUBLIC KEY:6�   �� 5^      
��� ���$��15(�6,   $��0, 0, 10, 0q�6:   $��Center=�6O   $
��Bold"�6j   $��Right=�6|   ��6�   �� *	PublicKey+5_      �� 5a   
   $��1��6   $��2��6%   $��0��69   $��2��6F   $��Center=�6X   $ 
Horizontal=�6s   $
��Left=�6�   .+�5b      �� 
�������$��15(�6'   $��Center=�65   $��
10,0,0,-30q�6P   $
��Left=�6d   .��6�   	DECRYPTED6�   5c      �� 5e   
   $��0��6   $��1��6   ���	#3FFFFFFF$��25, 25, 25, 25q�6F   
������� �� TextWrapping$
 Wrap=�5f   @   {�. ��6   �� *StringToEncrypt+ �� Mode$ TwoWay=�.��6e   5g      �� 5h   
   $��0��6   $��2��6   ���	#3FFFFFFF$��25, 25, 25, 25q�6F   
�������$
 Wrap=�5i   @   {�. ��6   �� *EncryptedString+$ TwoWay=�.��6e   5j      �� 5k   
   $��1��6   $��1��6   $��2��6/   ���	#3FFFFFFF$��25, 25, 25, 25q�6Z   
�������$
 Wrap=�5l   3   {�. ��6   �� *DecryptedString+.��6X   5m      5n      5o      