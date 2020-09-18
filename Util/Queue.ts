/*

Queue.ts

A reimplementation of Queue.js: A function to represent a queue

Created by Kate Morley - http://code.iamkate.com/ - and released under the terms
of the CC0 1.0 Universal legal code:

http://creativecommons.org/publicdomain/zero/1.0/legalcode

*/

/* Creates a new queue. A queue is a first-in-first-out (FIFO) data structure -
 * items are added to the end of the queue and removed from the front.
 */
export default class Queue<T> {

  // initialise the queue and offset
  private queue: T[] = [];
  private offset: number = 0;

  // Returns the length of the queue.
  public getLength(): number {
    return (this.queue.length - this.offset);
  }

  // Returns true if the queue is empty, and false otherwise.
  public isEmpty(): boolean {
    return (this.queue.length == 0);
  }

  /* Enqueues the specified item. The parameter is:
   *
   * item - the item to enqueue
   */
  public enqueue(item: T): void {
    this.queue.push(item);
  }

  /* Dequeues an item and returns it. If the queue is empty, the value
   * 'undefined' is returned.
   */
  public dequeue(): T {
    // if the queue is empty, return immediately
    if (this.queue.length == 0) return undefined;

    // store the item at the front of the queue
    var item = this.queue[this.offset];

    // increment the offset and remove the free space if necessary
    if (++this.offset * 2 >= this.queue.length) {
      this.queue = this.queue.slice(this.offset);
      this.offset = 0;
    }

    // return the dequeued item
    return item;
  }

  /* Returns the item at the front of the queue (without dequeuing it). If the
   * queue is empty then undefined is returned.
   */
  public peek(): T {
    return (this.queue.length > 0 ? this.queue[this.offset] : undefined);
  }
}
