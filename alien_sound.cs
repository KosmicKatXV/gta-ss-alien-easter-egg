  �alien_eeG� G^G\ d    �@� cleo\alien.ini�� Beacon_n       �� Beacon_s        �� Beacon_w        �� Beacon_e        , �
 cleo/ss/ev%d.ini  �
 generalctr    �
 �  x  �
 �  y �
 �  z �     � 
   �    
    M +����  9  M ���� ���� ���� ����     *8  
  � � � �  
    M ����  �  �   PVE ��D�z�@   A   A   AM �����	�
CLEO\cleo_audio\deadSilence.mp3� �
� �
� �
�   �>�	ALIEN4  `�  
� � � � � ALIEN3� � ���� R��� �����         �   B [       �   B [   �     �A � �      �
cleo/cleo_audio/loquendo.mp3� �
�  �
� �
� �
�   �?  � '�  � �    M ����� �
� �
� 	� ���� p ����     �
 �  d � V 8 �  �    
        M 
����
 �  volume �
 �  pck  ����
    M ���� ����  �  
    � �  9   M w����
cleo/cleo_audio/radio_alien.mp3� �
� � �
� �
� �
�    �  �    
        M p���� 9   �    
        M ~���
    �
 �  pck�
  generalctr�
� �            s�ALIEN1 � ��  9  M ~����ALIEN2 ���� w��� I��� �� �
 generalctr  �����
VAR    names ,   hMP3 <   hMP2 =   beacon ?   FLAG   SRC �  {$CLEO}
{$USE INI, CLEO+}
0000: NOP

script_name "alien_ee"
0247: load_model 162
0247: load_model 350
0247: load_model 348
:Load_coords
wait 100
float x,y,z,d,v
float d_action = 5.0
float x_arr[4]
float y_arr[4]
float z_arr[4]
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
    0AF2: z = read_float_from_ini_file info_ini section $names[i] key "z"
    x_arr[i] = x
    y_arr[i] = y
    z_arr[i] = z 
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
        00FE: actor $PLAYER_ACTOR sphere 1 in_sphere 3429.0 1215.0 6.64 radius 10.0 10.0 10.0 on_foot
    then
        01B6: force_weather 9
        0AAC: $hMP3 = load_audio_stream "CLEO\cleo_audio\deadSilence.mp3"
        0AAD: set_audio_stream $hMP3 state 1
        0AC0: audio_stream $hMP3 looped 1
        0ABC: set_audio_stream $hMP3 volume 0.25
        01F9: init_rampage_gxt 'ALIEN4' weapon 24 time_limit 60000 targets 10 target_models 162 162 162 162 completed_text 1
        00BA: show_text_styled GXT "ALIEN3" time 4000 style 4
        wait 2000 ms
        jump @frenzy
    end
end
jump @bcn_iterator

:frenzy
00A0: store_actor $PLAYER_ACTOR position_to x y z
0208: z = random_float_in_ranges -40.0 40.0
x += z
0208: z = random_float_in_ranges -40.0 40.0
y += z
02CE: z = ground_z_at x y 30.0
009A: 2@ = create_actor_pedtype 4 model 162 at x y z
0AC1: $hMP2 = load_audio_stream_with_3d_support "cleo/cleo_audio/loquendo.mp3"
0AC4: link_3d_audio_stream $hMP2 to_actor 2@
0AAD: set_audio_stream $hMP2 state 1
0AC0: audio_stream $hMP2 looped 1
0ABC: set_audio_stream $hMP2 volume 1.0
081A: set_actor 2@ weapon_skill_to 0
01B2: give_actor 2@ weapon 26 ammo 9999
05E2: AS_actor 2@ kill_actor $PLAYER_ACTOR
01FA: z = rampage_status
if
    z > 2
then
    Actor.RemoveReferences(2@)
    0AAE: remove_audio_stream $hMP3
    0AAE: remove_audio_stream $hMP2
    0915: sync_weather_with_time_and_location_instantly
    01B7: release_weather
    jump @blip_gen
end
wait 6000 ms
jump @frenzy

:bcn_iterator
wait 0 ms
FOR i = 0 TO 3
    0AF2: d = read_float_from_ini_file info_ini section $names[i] key "d"
    if and
        Player.Defined($PLAYER_CHAR)
        $ACTIVE_INTERIOR == 0 
        //$ONMISSION == 0 
        00FE: actor $PLAYER_ACTOR sphere 0 in_sphere x_arr[i] y_arr[i] z_arr[i] radius d d d
    then
        041E: set_radio_station 12
        0AF2: v = read_float_from_ini_file info_ini section $names[i] key "volume"
        0AF0: pck = read_int_from_ini_file save_ini section $names[i] key "pck"
        jump @sound
    end
end
jump @bcn_iterator
  


:sound
wait 0
0107: $beacon = create_object 3786 at x_arr[i] y_arr[i] z_arr[i]
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
    00FE: actor $PLAYER_ACTOR sphere 0 in_sphere x_arr[i] y_arr[i] z_arr[i] radius d d d
then
    041E: set_radio_station 12
    if and
        pck == 0
        00FE: actor $PLAYER_ACTOR sphere 0 in_sphere x_arr[i] y_arr[i] z_arr[i] radius d_action d_action d_action on_foot
    then
        ctr += 1
        pck = 1
        0AF1: write_int 1   to_ini_file save_ini section $names[i] key "pck"
        0AF1: write_int ctr to_ini_file save_ini section "general" key "ctr"
        0AAE: remove_audio_stream $hMP3
        018C: play_sound 1139 at 0.0 0.0 0.0
        01E3: show_text_1number_styled GXT "ALIEN1" number ctr time 5000 style 6
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

terminate_this_custom_script�  __SBFTR 