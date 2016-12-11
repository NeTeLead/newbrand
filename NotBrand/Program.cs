using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace NotBrand
{
    static class Program
    {
        static void Main(string[] args)
        {
           Loading.OnLoadingComplete += OnLoadingComplete;
        }

        public static AIHeroClient CurrentTarget;
        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Brand)
            {
                return;
            }

            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();

            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast2;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
        }
        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            CurrentTarget = TargetSelector.GetTarget(SpellManager.W.Range, DamageType.Magical);
            //(CurrentTarget.Hero != Champion.Yasuo && sender.Mana <= 90)//
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && sender == CurrentTarget && !sender.IsDashing() && sender.Type == GameObjectType.AIHeroClient && sender.IsValidTarget(SpellManager.W.Range) && SpellManager.W.IsReady() && sender.IsEnemy)
            {

                    if (CurrentTarget.Hero != Champion.Yasuo)
                    {
                    SpellManager.W.Cast(sender.ServerPosition);
                    //Chat.Print("Basic Attack:"+args.SData.Name);
                    }
                    else if (sender.Mana <= 90)
                    {
                    SpellManager.W.Cast(sender.ServerPosition);
                    }
                    
             }
            
        }
        private static void Obj_AI_Base_OnProcessSpellCast2(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //|| (CurrentTarget.Hero == Champion.Yasuo && sender.Mana >= 90)
            CurrentTarget = TargetSelector.GetTarget(SpellManager.W.Range + 500, DamageType.Magical);
            if (sender == null || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || (CurrentTarget.Hero == Champion.Yasuo && sender.Mana >= 90))
            {
               return;
            }
            if (SpellManager.W.IsReady() && !sender.IsInvulnerable && args.Target != CurrentTarget && !sender.IsDashing() && sender == CurrentTarget)
            {

                
                if (args.End.Distance(Player.Instance.Position) >= 100 || args.SData.TargettingType == SpellDataTargetType.Unit)
                {
                    if (Config.PunishMenu[args.SData.Name].Cast<CheckBox>().CurrentValue)
                    {
                        if (sender.IsValidTarget(900) && !Config.PunishSetupMenu[args.SData.Name].Cast<CheckBox>().CurrentValue)
                        {
                            //Chat.Print("Pos Cast:"+args.SData.Name);
                            SpellManager.W.Cast(sender.ServerPosition);
                        }
                        else if (args.End.Distance(Player.Instance.Position) <= 900 && Config.PunishSetupMenu[args.SData.Name].Cast<CheckBox>().CurrentValue)
                        {
                            //Chat.Print("End Cast:"+args.SData.Name);
                            SpellManager.W.Cast(args.End);
                        }  
                    }


                } 

            } 
        }
        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || e.End == Vector3.Zero || !e.End.IsInRange(user, SpellManager.Q.Range))
            {
                return;
            }


        }
        private static void OnDraw(EventArgs args)
        {
            if (Player.Instance.IsDead)
            {
                return;
            }

            if (Config.DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue && Player.GetSpell(SpellSlot.Q).IsLearned)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(new ColorBGRA(255, 0, 102, 255), SpellManager.Q.Range, Player.Instance);
            }

            if (Config.DrawMenu["drawW"].Cast<CheckBox>().CurrentValue && Player.GetSpell(SpellSlot.W).IsLearned)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(new ColorBGRA(255, 102, 0, 255), SpellManager.W.Range, Player.Instance);
            }

            if (Config.DrawMenu["drawE"].Cast<CheckBox>().CurrentValue && Player.GetSpell(SpellSlot.E).IsLearned)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(new ColorBGRA(204, 153, 0, 255), SpellManager.E.Range, Player.Instance);
            }

            if (Config.DrawMenu["drawR"].Cast<CheckBox>().CurrentValue && Player.GetSpell(SpellSlot.R).IsLearned)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(new ColorBGRA(204, 0, 0, 255), SpellManager.R.Range, Player.Instance);
            }
        }
    }
}
