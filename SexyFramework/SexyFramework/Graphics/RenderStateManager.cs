using System;
using System.Collections.Generic;

namespace SexyFramework.Graphics
{
	public abstract class RenderStateManager
	{
		public class StateValue
		{
			public enum EStateValueType
			{
				SV_Dword,
				SV_Float,
				SV_Ptr,
				SV_Vector
			}

			public EStateValueType mType;

			public uint mDword;

			public float mFloat;

			public object mPtr;

			public float mX;

			public float mY;

			public float mZ;

			public float mW;

			public StateValue()
			{
			}

			public StateValue(uint inDword)
			{
				mType = EStateValueType.SV_Dword;
				mDword = inDword;
			}

			public StateValue(float inFloat)
			{
				mType = EStateValueType.SV_Float;
				mFloat = inFloat;
			}

			public StateValue(object inPtr)
			{
				mType = EStateValueType.SV_Ptr;
				mPtr = inPtr;
			}

			public StateValue(float inX, float inY, float inZ, float inW)
			{
				mType = EStateValueType.SV_Vector;
				mX = inX;
				mY = inY;
				mZ = inZ;
				mW = inW;
			}

			public StateValue(StateValue inValue)
			{
				mType = inValue.mType;
				switch (mType)
				{
				case EStateValueType.SV_Dword:
					mDword = inValue.mDword;
					break;
				case EStateValueType.SV_Float:
					mFloat = inValue.mFloat;
					break;
				case EStateValueType.SV_Ptr:
					mPtr = inValue.mPtr;
					break;
				case EStateValueType.SV_Vector:
					mX = inValue.mX;
					mY = inValue.mY;
					mZ = inValue.mZ;
					mW = inValue.mW;
					break;
				}
			}

			public override bool Equals(object obj)
			{
				if (obj != null && obj is StateValue)
				{
					StateValue stateValue = (StateValue)obj;
					switch (mType)
					{
					case EStateValueType.SV_Dword:
						return mDword == stateValue.mDword;
					case EStateValueType.SV_Float:
						return mFloat == stateValue.mFloat;
					case EStateValueType.SV_Ptr:
						return mPtr == stateValue.mPtr;
					case EStateValueType.SV_Vector:
						if (mX == stateValue.mX && mY == stateValue.mY && mZ == stateValue.mZ)
						{
							return mW == stateValue.mW;
						}
						return false;
					default:
						return false;
					}
				}
				return false;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public uint GetDword()
			{
				return mDword;
			}

			public float GetFloat()
			{
				return mFloat;
			}

			public object GetPtr()
			{
				return mPtr;
			}

			public void GetVector(ref float outX, ref float outY, ref float outZ, ref float outW)
			{
				outX = mX;
				outY = mY;
				outZ = mZ;
				outW = mW;
			}

			public static bool operator ==(StateValue ImpliedObject, StateValue inValue)
			{
				return ImpliedObject?.Equals(inValue) ?? ((object)inValue == null);
			}

			public static bool operator !=(StateValue ImpliedObject, StateValue inValue)
			{
				return !(ImpliedObject == inValue);
			}
		}

		public class State
		{
			public delegate bool FCommitFunc(State inState);

			public RenderStateManager mManager;

			public uint[] mContext = new uint[4];

			public State mDirtyPrev;

			public State mDirtyNext;

			public StateValue mValue = new StateValue();

			public StateValue mHardwareDefaultValue = new StateValue();

			public StateValue mContextDefaultValue = new StateValue();

			public StateValue mLastCommittedValue = new StateValue();

			public State mContextDefPrev;

			public State mContextDefNext;

			public FCommitFunc mCommitFunc;

			public string mName;

			public State(RenderStateManager inManager, uint inContext0, uint inContext1, uint inContext2)
				: this(inManager, inContext0, inContext1, inContext2, 0u)
			{
			}

			public State(RenderStateManager inManager, uint inContext0, uint inContext1)
				: this(inManager, inContext0, inContext1, 0u, 0u)
			{
			}

			public State(RenderStateManager inManager, uint inContext0)
				: this(inManager, inContext0, 0u, 0u, 0u)
			{
			}

			public State(RenderStateManager inManager)
				: this(inManager, 0u, 0u, 0u, 0u)
			{
			}

			public State()
				: this(null, 0u, 0u, 0u, 0u)
			{
			}

			public State(RenderStateManager inManager, uint inContext0, uint inContext1, uint inContext2, uint inContext3)
			{
				mManager = inManager;
				mCommitFunc = null;
				mDirtyPrev = (mDirtyNext = this);
				mContextDefPrev = (mContextDefNext = this);
				mContext[0] = inContext0;
				mContext[1] = inContext1;
				mContext[2] = inContext2;
				mContext[3] = inContext3;
			}

			public State(State inState)
			{
				mManager = inState.mManager;
				mValue = new StateValue(inState.mValue);
				mHardwareDefaultValue = new StateValue(inState.mHardwareDefaultValue);
				mContextDefaultValue = new StateValue(inState.mContextDefaultValue);
				mLastCommittedValue = new StateValue(inState.mLastCommittedValue);
				mCommitFunc = inState.mCommitFunc;
				mDirtyPrev = (mDirtyNext = this);
				mContextDefPrev = (mContextDefNext = this);
				for (int i = 0; i < 4; i++)
				{
					mContext[i] = inState.mContext[i];
				}
			}

			public void Init(StateValue inDefaultValue, string inName)
			{
				Init(inDefaultValue, inName, null);
			}

			public void Init(ulong inDefaultValue, string inName)
			{
				Init(new StateValue(inDefaultValue), inName);
			}

			public void Init(StateValue inDefaultValue, string inName, string inValueEnumName)
			{
				mValue = (mHardwareDefaultValue = (mContextDefaultValue = (mLastCommittedValue = inDefaultValue)));
				mCommitFunc = mManager.GetCommitFunc(this);
				mName = inName;
			}

			public void Init(ulong inDefaultValue, string inName, string inValueEnumName)
			{
				Init(new StateValue(inDefaultValue), inName, inValueEnumName);
			}

			public void Init(StateValue inHardwareDefaultValue, StateValue inContextDefaultValue, string inName)
			{
				Init(inHardwareDefaultValue, inContextDefaultValue, inName, null);
			}

			public void Init(ulong inHardwareDefaultValue, ulong inContextDefaultValue, string inName)
			{
				Init(new StateValue(inHardwareDefaultValue), new StateValue(inContextDefaultValue), inName);
			}

			public void Init(StateValue inHardwareDefaultValue, StateValue inContextDefaultValue, string inName, string inValueEnumName)
			{
				mValue = (mHardwareDefaultValue = (mLastCommittedValue = inHardwareDefaultValue));
				mContextDefaultValue = inContextDefaultValue;
				mCommitFunc = mManager.GetCommitFunc(this);
				mName = inName;
				mContextDefPrev = mManager.mContextDefDummyHead;
				mContextDefNext = mManager.mContextDefDummyHead.mContextDefNext;
				mContextDefPrev.mContextDefNext = (mContextDefNext.mContextDefPrev = this);
			}

			public void Init(ulong inHardwareDefaultValue, ulong inContextDefaultValue, string inName, string inValueEnumName)
			{
				Init(new StateValue(inHardwareDefaultValue), new StateValue(inContextDefaultValue), inName, inValueEnumName);
			}

			public void Reset()
			{
				mLastCommittedValue = mHardwareDefaultValue;
			}

			public bool HasContextDefault()
			{
				return mContextDefPrev != this;
			}

			public bool IsDirty()
			{
				return mDirtyPrev != this;
			}

			public void SetDirty()
			{
				if (!IsDirty())
				{
					mDirtyPrev = mManager.mDirtyDummyHead;
					mDirtyNext = mManager.mDirtyDummyHead.mDirtyNext;
					mDirtyPrev.mDirtyNext = (mDirtyNext.mDirtyPrev = this);
					mManager.mWouldCommitStateDirty = true;
				}
			}

			public void ClearDirty()
			{
				ClearDirty(false);
			}

			public void ClearDirty(bool inActAsCommit)
			{
				if (IsDirty())
				{
					if (inActAsCommit)
					{
						mLastCommittedValue = mValue;
					}
					mDirtyPrev.mDirtyNext = mDirtyNext;
					mDirtyNext.mDirtyPrev = mDirtyPrev;
					mDirtyPrev = (mDirtyNext = this);
					mManager.mWouldCommitStateDirty = true;
				}
			}

			public void SetValue(StateValue inValue)
			{
				if (!(inValue == mValue))
				{
					mManager.Flush();
					if (mManager.mCurrentContext != null)
					{
						mManager.mCurrentContext.SplitChildren();
						mManager.mCurrentContext.mJournal.Add(new Context.JournalEntry(this, mValue, inValue));
					}
					mValue = inValue;
					SetDirty();
				}
			}

			public void SetValue(uint inDword)
			{
				SetValue(new StateValue(inDword));
			}

			public void SetValue(float inFloat)
			{
				SetValue(new StateValue(inFloat));
			}

			public void SetValue(IntPtr inPtr)
			{
				SetValue(new StateValue(inPtr));
			}

			public void SetValue(float inX, float inY, float inZ, float inW)
			{
				SetValue(new StateValue(inX, inY, inZ, inW));
			}

			public uint GetDword()
			{
				return mValue.GetDword();
			}

			public float GetFloat()
			{
				return mValue.GetFloat();
			}

			public object GetPtr()
			{
				return mValue.GetPtr();
			}

			public void GetVector(ref float outX, ref float outY, ref float outZ, ref float outW)
			{
				mValue.GetVector(ref outX, ref outY, ref outZ, ref outW);
			}
		}

		public class Context
		{
			public class JournalEntry
			{
				public State mState;

				public StateValue mOldValue = new StateValue();

				public StateValue mNewValue = new StateValue();

				public JournalEntry()
				{
					mState = null;
				}

				public JournalEntry(State inState, StateValue inOldValue, StateValue inNewValue)
				{
					mState = inState;
					mOldValue = inOldValue;
					mNewValue = inNewValue;
				}
			}

			public List<JournalEntry> mJournal = new List<JournalEntry>();

			public List<Context> mChildContexts = new List<Context>();

			public List<uint> mFloorStack = new List<uint>();

			public uint mJournalFloor;

			public Context mParentContext;

			public Context()
			{
				mParentContext = null;
				mJournalFloor = 0u;
				mParentContext = null;
			}

			public virtual void Dispose()
			{
				SplitChildren();
				if (mParentContext == null)
				{
					return;
				}
				int count = mParentContext.mChildContexts.Count;
				for (int i = 0; i < count; i++)
				{
					if (mParentContext.mChildContexts[i] == this)
					{
						mParentContext.mChildContexts.RemoveAt(i);
						break;
					}
				}
			}

			public void RevertState()
			{
			}

			public void PushState()
			{
			}

			public void PopState()
			{
			}

			public void Unacquire()
			{
				Unacquire(false);
			}

			public void Unacquire(bool inIgnoreParent)
			{
			}

			public void Reacquire()
			{
				Reacquire(false);
			}

			public void Reacquire(bool inIgnoreParent)
			{
			}

			public void SplitChildren()
			{
			}
		}

		protected State mDirtyDummyHead;

		protected State mContextDefDummyHead;

		protected Context mCurrentContext;

		protected Context mDefaultContext = new Context();

		protected bool mWouldCommitStateDirty;

		protected bool mWouldCommitStateResult;

		protected virtual State.FCommitFunc GetCommitFunc(State inState)
		{
			return null;
		}

		public RenderStateManager()
		{
			mDirtyDummyHead = new State();
			mContextDefDummyHead = new State();
			mWouldCommitStateDirty = false;
			mWouldCommitStateResult = false;
			mCurrentContext = mDefaultContext;
		}

		public virtual void Dispose()
		{
		}

		public abstract void Init();

		public abstract void Reset();

		public virtual void Cleanup()
		{
			mCurrentContext.Unacquire();
		}

		public bool IsDirty()
		{
			return mDirtyDummyHead.mDirtyNext != mDirtyDummyHead;
		}

		public void ApplyContextDefaults()
		{
			for (State mContextDefNext = mContextDefDummyHead.mContextDefNext; mContextDefNext != mContextDefDummyHead; mContextDefNext = mContextDefNext.mContextDefNext)
			{
				mContextDefNext.mValue = mContextDefNext.mContextDefaultValue;
				mContextDefNext.SetDirty();
			}
		}

		public bool WouldCommitState()
		{
			if (!mWouldCommitStateDirty)
			{
				return mWouldCommitStateResult;
			}
			mWouldCommitStateDirty = false;
			for (State mDirtyNext = mDirtyDummyHead.mDirtyNext; mDirtyNext != mDirtyDummyHead; mDirtyNext = mDirtyNext.mDirtyNext)
			{
				if (!(mDirtyNext.mValue == mDirtyNext.mLastCommittedValue) && mDirtyNext.mCommitFunc != null)
				{
					mWouldCommitStateResult = true;
					return mWouldCommitStateResult;
				}
			}
			mWouldCommitStateResult = false;
			return mWouldCommitStateResult;
		}

		public bool CommitState()
		{
			bool flag = true;
			while (mDirtyDummyHead.mDirtyNext != mDirtyDummyHead)
			{
				State mDirtyNext = mDirtyDummyHead.mDirtyNext;
				if (mDirtyNext.mValue == mDirtyNext.mLastCommittedValue)
				{
					mDirtyNext.ClearDirty();
					continue;
				}
				if (mDirtyNext.mCommitFunc != null)
				{
					flag &= mDirtyNext.mCommitFunc(mDirtyNext);
				}
				else
				{
					mDirtyNext.ClearDirty();
				}
				mDirtyNext.mLastCommittedValue = mDirtyNext.mValue;
			}
			return flag;
		}

		public virtual void Flush()
		{
		}

		public Context GetContext()
		{
			return mCurrentContext;
		}

		public void SetContext(Context inContext)
		{
			if (inContext == null)
			{
				inContext = mDefaultContext;
			}
			if (inContext != mCurrentContext)
			{
				if (mCurrentContext.mParentContext == inContext)
				{
					mCurrentContext.Unacquire(true);
					mCurrentContext = inContext;
				}
				else if (inContext.mParentContext == mCurrentContext)
				{
					mCurrentContext = inContext;
					mCurrentContext.Reacquire(true);
				}
				else
				{
					mCurrentContext.Unacquire();
					mCurrentContext = inContext;
					mCurrentContext.Reacquire();
				}
			}
		}

		public void RevertState()
		{
			mCurrentContext.RevertState();
		}

		public virtual void PushState()
		{
			mCurrentContext.PushState();
		}

		public virtual void PopState()
		{
			mCurrentContext.PopState();
		}
	}
}
