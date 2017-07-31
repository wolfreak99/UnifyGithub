// Original url: http://wiki.unity3d.com/index.php/Lock_Free_Queue
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/Lock_Free_Queue.cs
// File based on original modification date of: 19 December 2016, at 11:30. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.GeneralConcepts
{
Author: Fredrik Ludvigsen (Steinbitglis) 
Contents [hide] 
1 Description 
2 Usage 
3 Compatibility note 
4 C# - LockFreeQueue.cs 

Description This queue lets you simultaneously enqueue and dequeue items, even from multiple threads. 
Usage Instantiate a LockFreeQueue<T> for your specific data type, and use the queue to send stuff one way to / from other worker threads etc. 
Use two queues for two way communication with your custom threads. 
Compatibility note Interlocked.CompareExchange<T> does not play nice with AOT compilation, so be careful when working with consoles etc. 
C# - LockFreeQueue.cs using System.Collections.Generic;
 
public class SingleLinkNode<T> {
    // Note; the Next member cannot be a property since
    // it participates in many CAS operations
    public SingleLinkNode<T> Next;
    public T Item;
}
 
public static class SyncMethods {
    public static bool CAS<T>(ref T location, T comparand, T newValue) where T : class {
        return
            (object) comparand ==
            (object) Interlocked.CompareExchange<T>(ref location, newValue, comparand);
    }
}
 
public class LockFreeLinkPool<T> {
 
    private SingleLinkNode<T> head;
 
    public LockFreeLinkPool() {
        head = new SingleLinkNode<T>();
    }
 
    public void Push(SingleLinkNode<T> newNode) {
        newNode.Item = default(T);
        do {
            newNode.Next = head.Next;
        } while (!SyncMethods.CAS<SingleLinkNode<T>>(ref head.Next, newNode.Next, newNode));
        return;
    }
 
    public bool Pop(out SingleLinkNode<T> node) {
        do {
            node = head.Next;
            if (node == null) {
                return false;
            }
        } while (!SyncMethods.CAS<SingleLinkNode<T>>(ref head.Next, node, node.Next));
        return true;
    }
}
 
public class LockFreeQueue<T> {
 
    SingleLinkNode<T> head;
    SingleLinkNode<T> tail;
    LockFreeLinkPool<T> trash;
 
    public LockFreeQueue() {
        head = new SingleLinkNode<T>();
        tail = head;
        trash = new LockFreeLinkPool<T>();
    }
 
    public void Enqueue(T item) {
        SingleLinkNode<T> oldTail = null;
        SingleLinkNode<T> oldTailNext;
 
        SingleLinkNode<T> newNode;
        if (!trash.Pop(out newNode)) {
            newNode = new SingleLinkNode<T>();
        } else {
            newNode.Next = null;
        }
        newNode.Item = item;
 
        bool newNodeWasAdded = false;
        while (!newNodeWasAdded) {
            oldTail = tail;
            oldTailNext = oldTail.Next;
 
            if (tail == oldTail) {
                if (oldTailNext == null)
                    newNodeWasAdded = SyncMethods.CAS<SingleLinkNode<T>>(ref tail.Next, null, newNode);
                else
                    SyncMethods.CAS<SingleLinkNode<T>>(ref tail, oldTail, oldTailNext);
            }
        }
        SyncMethods.CAS<SingleLinkNode<T>>(ref tail, oldTail, newNode);
    }
 
    public bool Dequeue(out T item) {
        item = default(T);
        SingleLinkNode<T> oldHead = null;
 
        bool haveAdvancedHead = false;
        while (!haveAdvancedHead) {
 
            oldHead = head;
            SingleLinkNode<T> oldTail = tail;
            SingleLinkNode<T> oldHeadNext = oldHead.Next;
 
            if (oldHead == head) {
                if (oldHead == oldTail) {
                    if (oldHeadNext == null) {
                        return false;
                    }
                    SyncMethods.CAS<SingleLinkNode<T>>(ref tail, oldTail, oldHeadNext);
                } else {
                    item = oldHeadNext.Item;
                    haveAdvancedHead = SyncMethods.CAS<SingleLinkNode<T>>(ref head, oldHead, oldHeadNext);
                    if (haveAdvancedHead) {
                        trash.Push(oldHead);
                    }
                }
            }
        }
        return true;
    }
}
}
