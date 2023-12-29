  �alien_ee d    �@� cleo\alien.ini�� Beacon_n       �� Beacon_s        �� Beacon_w        �� Beacon_e        , �
 cleo/ss/ev%d.ini  �
 generalctr    �
 �  x  �
 �  y �     �    
    M :����  9  M ���� ���� ���� 6���     *8    � � � �  
    M ����  �  �   `UE  �D   A   M F���� D���� ��� 6���     �
 �  z �
 �  d � V 8 �  �          M �����
 �  volume �
 �  pck  ����
    M +��� 6���  �     � �  9   M ����
cleo/cleo_audio/radio_alien.mp3� �
� � �
� �
� �
�    �  �          M ���� 9   �          M !���
    �
 �  pck�
  generalctr�
� �            s�ALIEN1 � ��  9  M !����ALIEN2 ���� ��� ���� �� �
 generalctr  �����
VAR    names ,   beacon <   hMP3 =   FLAG   SRC �  {$CLEO}
{$USE INI, CLEO+}
0000: NOP

script_name "alien_ee"
:Load_coords
wait 100
float x,y,z,d,v,m_h,m_v,0@,1@,2@,3@
float d_action = 5.0
float x_arr[4]
float y_arr[4]
int i,i2,pck,ctr,slot
longstring save_ini
longstring info_ini = "cleo\alien.ini"
string $names[8]
$names[0] = 'Beacon_n'
$names[2] = 'Beacon_s'
$names[4] = 'Beacon_w'
$names[6] = 'Beacon_e'

0E2C: get_current_save_slot slot 
0AD3: save_ini = string_format "cleo/ss/ev%d.ini" slot
0AF0: ctr = read_int_from_ini_file save_ini section "general" key "ctr"

FOR i = 0 TO 3
    0AF2: x = read_float_from_ini_file info_ini section $names[i] key "x"
    0AF2: y = read_float_from_ini_file info_ini section $names[i] key "y"
    x_arr[i] = x
    y_arr[i] = y
end

:are_beacons_found
if 
    ctr == 4
then
    jump @blip_gen
else
    jump @bcn_iterator
end

:blip_gen
wait 0 ms
FOR i = 0 TO 3
    0E2A: add_cleo_blip 56 position x_arr[i] y_arr[i] is_short 1 RGBA 255 255 255 255 store_to 1@
end
while true
    wait 0 ms
    if
        00FE: actor $PLAYER_ACTOR sphere 1 in_sphere 3414.0 1200.0 10.0 radius d d d on_foot
    then
        01B5: force_weather 20
    else
        01B7: release_weather
    end
end
jump @bcn_iterator


:bcn_iterator
wait 0 ms
FOR i = 0 TO 3
    0AF2: z = read_float_from_ini_file info_ini section $names[i] key "z"
    0AF2: d = read_float_from_ini_file info_ini section $names[i] key "d"
    if and
        Player.Defined($PLAYER_CHAR)
        $ACTIVE_INTERIOR == 0 
        //$ONMISSION == 0 
        00FE: actor $PLAYER_ACTOR sphere 1 in_sphere x_arr[i] y_arr[i] z radius d d d on_foot
    then
        0AF2: v = read_float_from_ini_file info_ini section $names[i] key "volume"
        0AF0: pck = read_int_from_ini_file save_ini section $names[i] key "pck"
        jump @sound
    end
end
jump @bcn_iterator
  


:sound
wait 0
0107: $beacon = create_object 3786 at x_arr[i] y_arr[i] z
if
    pck == 0
then
    0AC1: $hMP3 = load_audio_stream_with_3d_support "cleo/cleo_audio/radio_alien.mp3"
    0AC3: link_3d_audio_stream $hMP3 to_object $beacon
    0AAD: set_audio_stream $hMP3 state 1
    0AC0: audio_stream $hMP3 looped 1
    0ABC: set_audio_stream $hMP3 volume v
end

:chk
wait 0 ms
if
    00FE: actor $PLAYER_ACTOR sphere 1 in_sphere x_arr[i] y_arr[i] z radius d d d on_foot
then
    if and
        pck == 0
        00FE: actor $PLAYER_ACTOR sphere 1 in_sphere x_arr[i] y_arr[i] z radius d_action d_action d_action on_foot
    then
        ctr += 1
        pck = 1
        0AF1: write_int 1   to_ini_file save_ini section $names[i] key "pck"
        0AF1: write_int ctr to_ini_file save_ini section "general" key "ctr"
        0AAE: remove_audio_stream $hMP3
        018C: play_sound 1139 at 0.0 0.0 0.0
        01E3: show_text_1number_styled GXT "ALIEN1" number ctr time 5000 style 6  // NEW HIGH SCORE!!~n~~w~~1~
        wait 5000 ms
        if
            ctr == 4
        then
            03E5: show_text_box "ALIEN2"
            jump @blip_gen
        end
    end
    jump @chk
else
    wait 1000 ms
    0108: destroy_object $beacon
    0AF0: ctr = read_int_from_ini_file save_ini section "general" key "ctr"
    jump @are_beacons_found
end

terminate_this_custom_script  __SBFTR 