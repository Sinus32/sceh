using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.BBCode;
using s32.Sceh.UserNoteTags.Factories;

namespace s32.Sceh.UserNoteTags
{
    public class UserNoteTagFactory
    {
        public static readonly UserNoteTagFactory Instance;
        private HashSet<IUserNoteTagFactory> _factories;

        static UserNoteTagFactory()
        {
            Instance = new UserNoteTagFactory();
            Instance.RegisterFactory(new DateTimeTagFactory());
            Instance.RegisterFactory(new SteamAppTagFactory());
            Instance.RegisterFactory(new SteamCardTagFactory());
        }

        public UserNoteTagFactory()
        {
            _factories = new HashSet<IUserNoteTagFactory>();
        }

        public IUserNoteTag CreateTag(IBBTagNode node)
        {
            foreach (var factory in _factories)
            {
                var result = factory.CreateTag(node);
                if (result != null)
                    return result;
            }
            return null;
        }

        public bool RegisterFactory(IUserNoteTagFactory factory)
        {
            return _factories.Add(factory);
        }
    }
}
