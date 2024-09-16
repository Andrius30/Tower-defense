//using System;


//namespace JustFunnyGames.Utilities
//{
//    public static class Events
//    {
//        public static void Notify(Action action, bool once = false)
//        {
//            if (action != null)
//            {
//                action();
//            }
//        }

//        public static void Notify<ActionArgType>(Action<ActionArgType> action, ActionArgType arg, bool once = false)
//        {
//            if (action != null)
//            {
//                action(arg);
                
//                if (once)
//                    action = null;
//            }
//        }

//        public static void Notify<ActionArgType1, ActionArgType2>(Action<ActionArgType1, ActionArgType2> action,
//            ActionArgType1 arg1, ActionArgType2 arg2)
//        {
//            if (action != null)
//            {
//                action(arg1, arg2);
//            }
//        }

//        public static void Notify<ActionArgType1, ActionArgType2, ActionArgType3>(
//            Action<ActionArgType1, ActionArgType2, ActionArgType3> action,
//            ActionArgType1 arg1, ActionArgType2 arg2, ActionArgType3 arg3)
//        {
//            if (action != null)
//            {
//                action(arg1, arg2, arg3);
//            }
//        }

//        public static void Notify<ActionArgType1, ActionArgType2, ActionArgType3, ActionArgType4>(
//            Action<ActionArgType1, ActionArgType2, ActionArgType3, ActionArgType4> action,
//            ActionArgType1 arg1, ActionArgType2 arg2, ActionArgType3 arg3, ActionArgType4 arg4)
//        {
//            if (action != null)
//            {
//                action(arg1, arg2, arg3, arg4);
//            }
//        }

//        public static void NotifyIfNeeded(bool needed, Action action)
//        {
//            if (needed && (action != null))
//            {
//                action();
//            }
//        }

//        public static void Notify<ActionArgType>(bool needed, Action<ActionArgType> action, ActionArgType arg)
//        {
//            if (needed && (action != null))
//            {
//                action(arg);
//            }
//        }

//        public static void Notify<ActionArgType1, ActionArgType2>(bool needed, Action<ActionArgType1, ActionArgType2> action,
//                                                                  ActionArgType1 arg1, ActionArgType2 arg2)
//        {
//            if (needed && (action != null))
//            {
//                action(arg1, arg2);
//            }
//        }

//        public static void Notify<ActionArgType1, ActionArgType2, ActionArgType3>(
//            bool needed, Action<ActionArgType1, ActionArgType2, ActionArgType3> action,
//            ActionArgType1 arg1, ActionArgType2 arg2, ActionArgType3 arg3)
//        {
//            if (needed && (action != null))
//            {
//                action(arg1, arg2, arg3);
//            }
//        }

//    }

//}
