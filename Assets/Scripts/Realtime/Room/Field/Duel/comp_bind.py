import abc
import json

from comp_bind import player_field, state

All_pose = ['vomtag', 'pflug', 'ochs', "alber"]
All_attack_type = ["uberhauw", "unterhau", "horizontal", "stechen"]

class card:
  __metaclass__ = abc.ABCMeta

  def __init__(self) -> None:
    self.card_code = None

    self.card_type = None         # 공격, 행동, 꺾인
    self.can_use_in_bind = False

    # attack
    self.attack_type = None       # 내려치기, 올려치기, 횡베기, 찌르기
    self.guard_type = None        # 내려치기, 올려치기, 횡베기, 찌르기
    self.damage = 0               # 데미지
    self.can_damage_decrease = True

    self.start_pose = None
    self.end_pose = None

    # guard
    self.guard_type = None
    self.guard_pose = None

    self.can_nullification = True

  def attack(self, state, user):
    return self.attack_effect.effect(state, user)

  def open(self):
    return self.open_effect.effect()

  def used(self):
    return self.used_effect.effect()
  
  def on_nullification(self):
    return self.on_nullification_effect.effect()
  
  def on_bind(self):
    return self.on_bind_effect.effect()
  
  def on_damaged(self):
    return self.on_damaged_effect.effect()

# 즈버크하우
class card_0_Zwerchhauw(card):
  def __init__(self) -> None:
    self.card_code = 0

    self.card_type = "attack"         # 공격, 행동, 꺾인
    self.can_use_in_bind = True

    # attack
    self.attack_type = "horizontal"       # 내려치기, 올려치기, 횡베기, 찌르기
    self.damage = 2               # 데미지
    self.can_damage_decrease = True

    self.start_pose = "ochs"
    self.end_pose = "ochs"

    # guard
    self.guard_attack = None
    self.guard_pose = "vomtag"

    self.can_nullification = True

    # effect
    self.attack_effect = Zwerchhauw_attack()
    self.open_effect = None_effect()
    self.used_effect = None_effect()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 크럼프하우    
class card_1_Krumphauw(card):
  def __init__(self) -> None:
    self.card_code = 1

    self.card_type = "action"         # 공격, 행동, 꺾인
    self.can_use_in_bind = False

    # attack
    self.attack_type = None       # 내려치기, 올려치기, 횡베기, 찌르기
    self.damage = None               # 데미지
    self.can_damage_decrease = None

    self.start_pose = "vomtag"
    self.end_pose = "pflug"

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = Krumphauw_open()
    self.used_effect = Krumphauw_used()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 존하우
class card_2_Zornhauw(card):
  def __init__(self) -> None:
    self.card_code = 2

    self.card_type = "attack"         # 공격, 행동, 꺾인
    self.can_use_in_bind = False

    # attack
    self.attack_type = "uberhauw"       # 내려치기, 올려치기, 횡베기, 찌르기
    self.damage = 4               # 데미지
    self.can_damage_decrease = False

    self.start_pose = "vomtag"
    self.end_pose = "alber"

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = None_effect()
    self.used_effect = None_effect()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 쉴하우
class card_3_Schielhauw(card):
  def __init__(self) -> None:
    self.card_code = 3

    self.card_type = "attack"         # attack, action, break
    self.can_use_in_bind = False

    # attack
    self.attack_type = "stechen"      # uberhauw, unterhau, horizontal, stechen
    self.damage = 2
    self.can_damage_decrease = True

    self.start_pose = "vomtag"        # vomtag, pflug, ochs, alber
    self.end_pose = ["pflug", "ochs"]

    # guard
    self.guard_attack = ["uberhauw", "stechen"]
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = None_effect()
    self.used_effect = None_effect()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 랑오트
class card_4_Langenort(card): # ********************** 이름 찾기
  def __init__(self) -> None:
    self.card_code = 4

    self.card_type = "attack"         # 공격, 행동, 꺾인
    self.can_use_in_bind = False

    # attack
    self.attack_type = "stechen"       # 내려치기, 올려치기, 횡베기, 찌르기
    self.damage = 2               # 데미지
    self.can_damage_decrease = True

    self.start_pose = ["pflug", "ochs"]
    self.end_pose = ["pflug", "ochs"]

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = None_effect()
    self.used_effect = None_effect()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 샤이텔하우
class card_5_Scheitelhauw(card):
  def __init__(self) -> None:
    self.card_code = 5

    self.card_type = "attack"         # 공격, 행동, 꺾인
    self.can_use_in_bind = False

    # attack
    self.attack_type = "uberhauw"       # 내려치기, 올려치기, 횡베기, 찌르기
    self.damage = 3               # 데미지
    self.can_damage_decrease = True

    self.start_pose = "vomtag"
    self.end_pose = ["pflug", "alber"]

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = Scheitelhauw_attack()
    self.open_effect = None_effect()
    self.used_effect = None_effect()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 아우프슈트라이첸
class card_6_아우프슈트라이첸(card): #**********************
  def __init__(self) -> None:
    self.card_code = 6

    self.card_type = "attack"         # 공격, 행동, 꺾인
    self.can_use_in_bind = False

    # attack
    self.attack_type = "unterhau"       # 내려치기, 올려치기, 횡베기, 찌르기
    self.damage = 3               # 데미지
    self.can_damage_decrease = True

    self.start_pose = "alber"
    self.end_pose = "ochs"

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = 아우프_attack() #***************
    self.open_effect = None_effect()
    self.used_effect = None_effect()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 듀플리에렌
class card_7_듀플(card):
  def __init__(self) -> None:
    self.card_code = 7

    self.card_type = "action"         # 공격, 행동, 꺾인
    self.can_use_in_bind = True

    # attack
    self.attack_type = None       # 내려치기, 올려치기, 횡베기, 찌르기
    self.damage = None               # 데미지
    self.can_damage_decrease = True

    self.start_pose = None
    self.end_pose = None

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = 듀플_open()
    self.used_effect = 듀플_used()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 존훗
class card_8_Zornhut(card):
  def __init__(self) -> None:
    self.card_code = 8

    self.card_type = "action"         # attack, action, break
    self.can_use_in_bind = False

    # attack
    self.attack_type = None      # uberhauw, unterhauw, horizontal, stechen
    self.damage = None
    self.can_damage_decrease = None

    self.start_pose = None        # vomtag, pflug, ochs, alber
    self.end_pose = None

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = Zornhut_open()
    self.used_effect = Zornhut_used()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 뮤티에렌
class card_9_뮤티에렌(card):
  def __init__(self) -> None:
    self.card_code = 9

    self.card_type = "attack"         # attack, action, break
    self.can_use_in_bind = True

    # attack
    self.attack_type = "uberhauw"      # uberhauw, unterhauw, horizontal, stechen
    self.damage = 3
    self.can_damage_decrease = True

    self.start_pose = All_pose        # vomtag, pflug, ochs, alber
    self.end_pose = "ochs"

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = False

    # effect
    self.attack_effect = None_effect()
    self.open_effect = None_effect()
    self.used_effect = None_effect()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 뷘든
class card_10_Winden(card):
  def __init__(self) -> None:
    self.card_code = 10

    self.card_type = "action"         # attack, action, break
    self.can_use_in_bind = False

    # attack
    self.attack_type = None      # uberhauw, unterhauw, horizontal, stechen
    self.damage = None
    self.can_damage_decrease = None

    self.start_pose = None        # vomtag, pflug, ochs, alber
    self.end_pose = None

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = Winden_open()
    self.used_effect = Winden_used()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 벨퓌렌
class card_11_벨퓌렌(card):
  def __init__(self) -> None:
    self.card_code = 11

    self.card_type = "attack"         # attack, action, break
    self.can_use_in_bind = False

    # attack
    self.attack_type = "uberhauw"      # uberhauw, unterhauw, horizontal, stechen
    self.damage = 2
    self.can_damage_decrease = True

    self.start_pose = "vomtag"        # vomtag, pflug, ochs, alber
    self.end_pose = "alber"

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = None_effect()
    self.used_effect = None_effect()
    self.on_nullification_effect = 벨퓌렌_on_nullification()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 행앤
class card_12_Hengen(card):
  def __init__(self) -> None:
    self.card_code = 12

    self.card_type = "action"         # attack, action, break
    self.can_use_in_bind = True

    # attack
    self.attack_type = None      # uberhauw, unterhauw, horizontal, stechen
    self.damage = None
    self.can_damage_decrease = None

    self.start_pose = None        # vomtag, pflug, ochs, alber
    self.end_pose = None

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = Hengen_open()
    self.used_effect = Hengen_used()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 두히벡셀
class card_13_두히벡셀(card):
  def __init__(self) -> None:
    self.card_code = 13

    self.card_type = "attack"         # attack, action, break
    self.can_use_in_bind = False

    # attack
    self.attack_type = "stechen"      # uberhauw, unterhauw, horizontal, stechen
    self.damage = 1
    self.can_damage_decrease = True

    self.start_pose = ["pflug", "ochs"]        # vomtag, pflug, ochs, alber
    self.end_pose = ["pflug", "ochs"]

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = None_effect()
    self.used_effect = None_effect()
    self.on_nullification_effect = 두히벡셀_on_nullification_or_bind()
    self.on_bind_effect = 두히벡셀_on_nullification_or_bind()
    self.on_damaged_effect = None_effect()

# 슐뤼셀
class card_14_Schlussel(card):
  def __init__(self) -> None:
    self.card_code = 14

    self.card_type = "action"         # attack, action, break
    self.can_use_in_bind = False

    # attack
    self.attack_type = None      # uberhauw, unterhauw, horizontal, stechen
    self.damage = None
    self.can_damage_decrease = None

    self.start_pose = None        # vomtag, pflug, ochs, alber
    self.end_pose = None

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = Schlussel_open()
    self.used_effect = None_effect()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = Schlussel_on_damaged()

# 자세변환: 폼탁
class card_15_vomtag(card):
  def __init__(self) -> None:
    self.card_code = 15
  
    self.card_type = "break"         # attack, action, break
    self.can_use_in_bind = False

    # attack
    self.attack_type = None      # uberhauw, unterhauw, horizontal, stechen
    self.damage = None
    self.can_damage_decrease = None

    self.start_pose = None        # vomtag, pflug, ochs, alber
    self.end_pose = None

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = False

    # effect
    self.attack_effect = None_effect()
    self.open_effect = vomtag_open()
    self.used_effect = None_effect()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()
    
# 자세변환: 플루크
class card_16_pflug(card):
  def __init__(self) -> None:
    self.card_code = 16

    self.card_type = "break"         # attack, action, break
    self.can_use_in_bind = False

    # attack
    self.attack_type = None      # uberhauw, unterhauw, horizontal, stechen
    self.damage = None
    self.can_damage_decrease = None

    self.start_pose = None        # vomtag, pflug, ochs, alber
    self.end_pose = None

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = False

    # effect
    self.attack_effect = None_effect()
    self.open_effect = pflug_open()
    self.used_effect = None_effect()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 자세변환: 옥스
class card_17_ochs(card):
  def __init__(self) -> None:
    self.card_code = 17

    self.card_type = "break"         # attack, action, break
    self.can_use_in_bind = False

    # attack
    self.attack_type = None      # uberhauw, unterhauw, horizontal, stechen
    self.damage = None
    self.can_damage_decrease = None

    self.start_pose = None        # vomtag, pflug, ochs, alber
    self.end_pose = None

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = False

    # effect
    self.attack_effect = None_effect()
    self.open_effect = ochs_open()
    self.used_effect = None_effect()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 주켄
class card_18_Zucken(card):
  def __init__(self) -> None:
    self.card_code = 18

    self.card_type = "break"         # attack, action, break
    self.can_use_in_bind = True

    # attack
    self.attack_type = None      # uberhauw, unterhauw, horizontal, stechen
    self.damage = None
    self.can_damage_decrease = None

    self.start_pose = None        # vomtag, pflug, ochs, alber
    self.end_pose = None

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = None_effect()
    self.used_effect = Zucken_used()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

# 공격: 옥스
class card_19_attack_ochs(card):
  def __init__(self) -> None:
    self.card_code = 19

    self.card_type = "attack"         # attack, action, break
    self.can_use_in_bind = False

    # attack
    self.attack_type = "unterhauw"      # uberhauw, unterhauw, horizontal, stechen
    self.damage = 2
    self.can_damage_decrease = True

    self.start_pose = None        # vomtag, pflug, ochs, alber
    self.end_pose = None

    # guard
    self.guard_attack = None
    self.guard_pose = None

    self.can_nullification = True

    # effect
    self.attack_effect = None_effect()
    self.open_effect = None_effect()
    self.used_effect = None_effect()
    self.on_nullification_effect = None_effect()
    self.on_bind_effect = None_effect()
    self.on_damaged_effect = None_effect()

class cardZone:
  def __init__(self, data) -> None:
    self.card = data['card']
    self.Deactivation = data['Deactivation']
    self.add_damage_type = data['add_damage_type']
    self.add_damage = data['add_damage']
    self.can_use_Zwerch = data['can_use_Zwerch']
    self.real_damage = 0
    self.nullification = False
    self.using_used_effect = True

class player:
  def __init__(self, data) -> None:
    self.hp = data['hp']
    self.pose = data['pose']
    self.order = data['order']

class player_field:
  def __init__(self, data) -> None:
    self.player: player = player(data['player'])
    self.cards = [cardZone(data['cards'][0]), cardZone(data['cards'][1]), cardZone(data['cards'][2])]

class state:
  def __init__(self, data) -> None:
    self.data = data
    data = json.loads(data)
    self.bind = data['bind']
    self.turn = data['turn']
    self.first_player_field: player_field = player_field(data['first_player_field'])
    self.second_player_field: player_field = player_field(data['first_player_field'])
  
  # def copy(self):
  #   return state(self.data)

# abstract effect
class effect:
  __metaclass__ = abc.ABCMeta

  @abc.abstractmethod
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state: state) -> None:
    pass

def order_decorator(f):
  def wrapper(self, state: state, user):
    if user == 0:
      use_player_field = state.first_player_field
      enemy_player_field = state.second_player_field
    elif user == 1:
      use_player_field = state.second_player_field
      enemy_player_field = state.first_player_field

    return f(self, use_player_field, enemy_player_field, state)
  return wrapper

# effects
class None_effect(effect):
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    pass

class Zwerchhauw_attack(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    if state.turn < 2:
      use_player_field.cards[state.turn+1].can_use_Zwerch = True

class Krumphauw_open(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    if enemy_player_field.cards[state.turn].real_damage <= 2:
      enemy_player_field.cards[state.turn].nullification = True
    else:
      use_player_field.cards[state.turn].using_used_effect = False

class Krumphauw_used(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    if state.turn < 2:
      enemy_player_field.cards[state.turn + 1].Deactivation = True

class Scheitelhauw_attack(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    if cards[enemy_player_field.cards[state.turn].card].attack_type == "uberhauw":
      enemy_player_field.cards[state.turn].nullification = True

class 아우프_attack(effect): #**********
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    if state.turn > 0 and cards[use_player_field.cards[state.turn-1].card].card_type == "attack":
      use_player_field.cards[state.turn].nullification = True

class 듀플_open(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    if state.bind and cards[enemy_player_field.cards[state.turn]].card_type == "attack":
      enemy_player_field.cards[state.turn].Deactivation = True

class 듀플_used(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    if state.turn > 0:
      use_player_field.cards[state.turn].card = use_player_field.cards[state.turn-1].card + 100

class Zornhut_open(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    if cards[enemy_player_field.cards[state.turn].card].attack_type in ["uberhauw", "unterhauw"] and\
       cards[enemy_player_field.cards[state.turn].card].can_damage_decrease:
      enemy_player_field.cards[state.turn].real_damage = -2
      use_player_field.player.pose = "vomtag"

class Zornhut_used(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    if state.turn < 2:
      use_player_field.cards[state.turn + 1].add_damage = 1
      use_player_field.cards[state.turn + 1].add_damage_type = All_attack_type

class Winden_open(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    if cards[enemy_player_field.cards[state.turn]].card_type in ["action", "break"]:
      enemy_player_field.cards[state.turn].nullification = True

class Winden_used(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    use_player_field.player.pose = enemy_player_field.player.pose
    state.bind = None # bind 상태였던 경우에도 bind가 되었다는것을 표시하기 위함

class 벨퓌렌_on_nullification(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    use_player_field.cards[state.turn].nullification = False
    use_player_field.player.pose = All_pose

class Hengen_open(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    if cards[enemy_player_field.cards[state.turn].card].attack_type == "uberhauw":
      if state.bind:
        enemy_player_field.cards[state.turn].Deactivation = True
      else:
        enemy_player_field.cards[state.turn].nullification = True
    else:
      use_player_field.cards[state.turn].using_used_effect = False

class Hengen_used(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    use_player_field.player.pose = All_pose

class 두히벡셀_on_nullification_or_bind(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state) -> None:
    state.bind = False
    use_player_field.cards[state.turn].nullification = False
    use_player_field.cards[state.turn].real_damage += 2

class Schlussel_open(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state: state) -> None:
    use_player_field.player.pose = "pflug"
    if state.turn < 2:
      use_player_field.cards[state.turn + 1].add_damage = 2
      use_player_field.cards[state.turn + 1].add_damage_type = "stechen"

class Schlussel_on_damaged(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state: state) -> None:
    enemy_player_field.player.hp -= 1

class vomtag_open(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state: state) -> None:
    use_player_field.player.pose = "vomtag"

class pflug_open(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state: state) -> None:
    use_player_field.player.pose = "pflug"

class ochs_open(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state: state) -> None:
    use_player_field.player.pose = "ochs"
    use_player_field.cards[state.turn].card = 19

class Zucken_used(effect):
  @order_decorator
  def effect(self, use_player_field: player_field, enemy_player_field: player_field, state: state) -> None:
    if state.turn < 2:
      enemy_player_field.cards[state.turn + 1].Deactivation = True


cards = [card_0_Zwerchhauw(), card_1_Krumphauw(), card_2_Zornhauw(), card_3_Schielhauw(), card_4_Langenort(), card_5_Scheitelhauw(),
         card_6_아우프슈트라이첸(), card_7_듀플(), card_8_Zornhut(), card_9_뮤티에렌, card_10_Winden(), card_11_벨퓌렌(), card_12_Hengen(),
         card_13_두히벡셀(), card_14_Schlussel(), card_15_vomtag(), card_16_pflug(), card_17_ochs(), card_18_Zucken(), card_19_attack_ochs()]

class comper:
  def __init__(self, first_card: int, second_card: int, cur_state: state) -> None:
    self.first_card: card = cards[first_card]
    self.second_card: card = cards[second_card]
    self.state: state = cur_state
  
  def comp(self):
    turn: int = self.state.turn
    binder = "second"

    # calc real damage
    cur: cardZone = self.state.first_player_field.cards[turn]
    if cur.add_damage > 0 and cards[cur.card].attack_type in cur.add_damage_type:
      cur.real_damage = cur.add_damage + cards[cur.card].damage
    else:
      cur.real_damage = cards[cur.card].damage
    
    cur: cardZone = self.state.second_player_field.cards[turn]
    if cur.add_damage > 0 and cards[cur.card].attack_type in cur.add_damage_type:
      cur.real_damage = cur.add_damage + cards[cur.card].damage
    else:
      cur.real_damage = cards[cur.card].damage

    # effect

    # 선공비활성화 검사
    if not self.state.first_player_field.cards[turn].Deactivation:

      # 선공 공개 / 공격효과
      self.first_card.open(self.state, 0)
      self.first_card.attack(self.state, 0)

      # 후공 무효화시 효과
      if self.state.second_player_field.cards[turn].nullification:
        self.second_card.on_nullification(self.state, 1)
    
    # 후공 비활성화 / 무효화 검사
    if (not self.state.second_player_field.cards[turn].Deactivation) and not \
      (self.second_card.can_nullification and self.state.second_player_field.cards[turn].nullification):

      # 후공 공개 / 공격효과
      self.second_card.open(self.state, 1)
      self.second_card.attack(self.state, 1)

      # 선공 무효화시 효과
      if self.state.first_player_field.cards[turn].nullification:
        self.first_card.on_nullification(self.state, 0)

    # 바인드
    if self.first_card.card_code == self.second_card.card_code:
      self.state.bind = None
      binder = "second"
      self.first_card.on_bind(self.state, 0)
      self.second_card.on_bind(self.state, 1)
    elif self.first_card.attack_type in self.second_card.guard_type or\
         self.state.first_player_field.player.pose in self.second_card.guard_pose:
      self.state.bind = None
      binder = "second"
      self.first_card.on_bind(self.state, 0)
      self.second_card.on_bind(self.state, 1)
    elif self.second_card.attack_type in self.first_card.guard_type or\
         self.state.second_player_field.player.pose in self.first_card.guard_pose:
      self.state.bind = None
      binder = "first"
      self.first_card.on_bind(self.state, 0)
      self.second_card.on_bind(self.state, 1)
      
    if self.state.second_player_field.player.pose == "pflug":
      self.state.first_player_field.cards[turn].real_damage -= 1
      
    if self.state.first_player_field.player.pose == "pflug":
      self.state.second_player_field.cards[turn].real_damage -= 1
    
    if not self.state.bind:
      # 데미지 계산
      if self.first_card.card_type == "attack" and self.state.first_player_field.cards[turn].real_damage > 0 and\
         (not self.state.first_player_field.cards[turn].Deactivation) and\
         (not self.state.first_player_field.cards[turn].nullification):
        self.state.second_player_field.player.hp -= self.state.first_player_field.cards[turn].real_damage
        self.state.second_player_field.cards[turn].using_used_effect = False
        self.state.second_player_field.cards[turn].Deactivation = True
      elif self.second_card.card_type == "attack" and self.state.second_player_field.cards[turn].real_damage > 0 and\
           (not self.state.second_player_field.cards[turn].Deactivation) and\
           (not self.state.second_player_field.cards[turn].nullification):
        self.state.first_player_field.player.hp -= self.state.second_player_field.cards[turn].real_damage
        self.state.first_player_field.cards[turn].using_used_effect = False
        self.state.first_player_field.cards[turn].Deactivation = True
      
      # used effect
      if (not self.state.first_player_field.cards[turn].Deactivation) and\
         (not self.state.first_player_field.cards[turn].nullification) and\
         self.state.first_player_field.cards[turn].using_used_effect:
        self.first_card.used(self.state, 0)

      if (not self.state.second_player_field.cards[turn].Deactivation) and\
         (not self.state.second_player_field.cards[turn].nullification) and\
         self.state.second_player_field.cards[turn].using_used_effect:
        self.second_card.used(self.state, 1)

      # 듀플리에렌으로 복사한 경우
      if self.state.first_player_field.cards[turn].card >= 100 or self.state.second_player_field.cards[turn].card >= 100:
        if self.state.second_player_field.cards[turn].card < 100:
          # 선공만 듀플
          self.state.first_player_field.cards[turn].card = self.state.first_player_field.cards[turn].card - 100
          self.first_card: card = cards[self.state.first_player_field.cards[turn].card - 100]
          self.first_card.open(self.state, 0)
          self.first_card.attack(self.state, 0)
          self.first_card.used(self.state, 0)

        elif self.state.first_player_field.cards[turn].card < 100:
          # 후공만 듀플
          self.state.second_player_field.cards[turn].card = self.state.second_player_field.cards[turn].card - 100
          self.second_card: card = cards[self.state.second_player_field.cards[turn].card - 100]
          self.second_card.open(self.state, 0)
          self.second_card.attack(self.state, 0)
          self.second_card.used(self.state, 0)
        else:
          f = self.state.first_player_field.cards[turn].card = self.state.first_player_field.cards[turn].card - 100
          s = self.state.second_player_field.cards[turn].card = self.state.second_player_field.cards[turn].card - 100

          return comper(f, s, self.state).comp()


    else:
      if binder == "second":
        self.state.first_player_field.player.order = False
        self.state.second_player_field.player.order = True

    
    if self.state.bind == True:
      self.state.bind = False
    if self.state.bind == None:
      self.state.bind = True
    
    if self.state.first_player_field.cards[turn].card == 19:
      self.state.first_player_field.cards[turn].card = 17
      
    if self.state.second_player_field.cards[turn].card == 19:
      self.state.second_player_field.cards[turn].card = 17
    
    return self.state
